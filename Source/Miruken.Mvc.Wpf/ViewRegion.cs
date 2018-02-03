namespace Miruken.Mvc.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Animation;
    using Callback;
    using Concurrency;
    using Context;
    using Infrastructure;
    using Map;
    using Mvc.Animation;
    using Options;
    using Views;

    public class ViewRegion : ViewContainer, IViewStackView
    {
        private bool _unwinding;

        public ViewRegion()
        {
            Layers = new List<ViewLayer>();
            Policy.OnRelease(UnwindLayers);
        }

        private List<ViewLayer> Layers { get; }

        private ViewController ActiveView
        {
            get
            {
                var activeLayer = ActiveLayer;
                return activeLayer?.View;
            }
        }

        protected override IViewLayer Show(IView view, IHandler composer)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return TransitionTo(view, composer);
        }

        private IViewLayer TransitionTo(IView view, IHandler composer)
        {
            var options       = GetRegionOptions(composer);
            var windowOptions = options?.Window;
            if (windowOptions != null)
                return CreateWindow(windowOptions, view, composer);

            var       push    = false;
            var       overlay = false;
            ViewLayer layer   = null;

            var layerOptions = options?.Layer;

            if (Layers.Count == 0)
                push = true;
            else if (layerOptions != null)
            {
                if (layerOptions.Push == true)
                    push = true;
                else if (layerOptions.Overlay == true)
                {
                    push = overlay = true;
                }
                else if (layerOptions.Unload == true)
                {
                    UnwindLayers();
                    push = true;
                }
                else
                    layer = (ViewLayer) layerOptions.Choose(
                        Layers.Cast<IViewLayer>().ToArray());
            }

            if (push)
            {
                var pop     = overlay ? PushOverlay() : PushLayer();
                var context = composer.Resolve<IContext>();
                if (context != null)
                    context.ContextEnding += _ => pop.Dispose();
            }

            if (layer == null) layer = ActiveLayer;
            return layer.TransitionTo(view, options, composer);
        }

        protected virtual IViewLayer CreateWindow(
            WindowOptions options, IView content, IHandler composer)
        {
            ViewRegion        region;
            NavigationRequest navigation = null;

            var owner = Window.GetWindow(this);
            if (owner == null)  // adopt region
                region = this;
            else
            {
                navigation = EnsureCompatibleNavigation(composer);
                region     = new ViewRegion();
            }
            var window = CreateWindow(owner, options, region);
            IHandler handler = null;
            if (ReferenceEquals(region, this))
                handler = composer;
            else if (navigation?.Style == NavigationStyle.Push)
            {
                var context = composer.Resolve<IContext>();
                context.AddHandlers(region);
                handler = composer;
            }
            if (handler == null)
                handler = new HandlerAdapter(region).Chain(composer);
            var layer = handler.SuppressWindows().Proxy<IViewRegion>().Show(content);
            layer.Disposed += (s, e) =>
            {
                if (window.Dispatcher.CheckAccess())
                    window.Close();
                else
                    window.Dispatcher.Invoke(new ThreadStart(window.Close));
            };
            window.Closed += (s, e) => layer.Dispose();
            switch (options.FillScreen)
            {
                case ScreenFill.FullScreen:
                case ScreenFill.VirtualScreen:
                case ScreenFill.SplitTop:
                case ScreenFill.SplitLeft:
                case ScreenFill.SplitRight:
                case ScreenFill.SplitBottom:
                    window.WindowStyle = WindowStyle.None;
                    window.ResizeMode  = ResizeMode.NoResize;
                    break;
                case ScreenFill.CenterParent:
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    break;
                case ScreenFill.CenterScreen:
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    break;
            }

            var frame = options.GetFrame(window.GetFrame());
            if (frame.HasValue)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.SetFrame(frame.Value);
            }

            if (options.Modal == true)
                window.ShowDialog();
            else
                window.Show();
            return layer;
        }

        private static Window CreateWindow(
            Window owner, WindowOptions options, ViewRegion region)
        {
            Window window;
            var windowType = options.WindowType;
            if (options.Readonly == true)
            {
                if (options.Modal == true)
                    throw new InvalidOperationException("Readonly windows can't be modal");
                if (windowType != null)
                    throw new InvalidOperationException("Readonly windows can't have custom type");
                window = new ReadonlyWindow();
            }
            else if (windowType != null)
            {
                if (!typeof(Window).IsAssignableFrom(windowType))
                    throw new InvalidOperationException(
                        $"{windowType.FullName} must extend System.Windows.Window");
                window = (Window)Activator.CreateInstance(windowType);
            }
            else
                window = new Window();

            if (options.Name != null)
                window.Name = options.Name;

            if (options.Title != null)
                window.Title = options.Title;

            window.Content = region;
            if (options.Standalone != true && owner != null)
            {
                window.ShowInTaskbar = false;
                if (!owner.IsVisible) owner.Show();
                window.Owner = owner;
            }

            if (options.HideCursor == true)
                window.Cursor = Cursors.None;

            return window;
        }

        private static NavigationRequest EnsureCompatibleNavigation(IHandler composer)
        {
            var navigation = composer.Resolve<NavigationRequest>();
            if (navigation?.Style == NavigationStyle.Next)
                throw new InvalidOperationException(
                    $"{navigation.ControllerType.FullName}::{navigation.Action.Name} is presenting a new Window, but has no matching context.  Did you do a Next instead of a Push?");
            return navigation;
        }

        #region Layer Methods

        private ViewLayer ActiveLayer => 
            Layers.Count > 0 ? Layers[Layers.Count - 1]  : null;

        public IDisposable PushLayer()
        {
            return CreateLayer(false);
        }

        public IDisposable PushOverlay()
        {
            return CreateLayer(true);
        }

        public void UnwindLayers()
        {
            _unwinding = true;
            while (Layers.Count > 0)
                Layers.Last().Dispose();
            _unwinding = false;
        }

        private ViewLayer DropLayer(ViewLayer layer)
        {
            var index = Layers.IndexOf(layer);
            if (index <= 0) return null;
            Layers.RemoveAt(index);
            return Layers[index - 1];
        }

        private void RemoveLayer(ViewLayer layer)
        {
            Layers.Remove(layer);
            layer.TransitionFrom();
        }

        private ViewLayer CreateLayer(bool overlay)
        {
            var layer = new ViewLayer(this, overlay);
            Layers.Add(layer);
            return layer;
        }

        private ViewLayer GetLayerBelow(ViewLayer layer)
        {
            var index = GetLayerIndex(layer);
            return index > 0 ? Layers[index - 1] : null;
        }

        private int GetLayerIndex(ViewLayer layer)
        {
            return Layers.IndexOf(layer);
        }

        #endregion

        #region Helper Methods

        private Promise AddView(ViewController fromView,
            ViewController view, RegionOptions options,
            bool removeFromView, IHandler composer)
        {
            if (!Dispatcher.CheckAccess())
                return (Promise)Dispatcher.Invoke(
                    new Func<ViewController, ViewController,
                    RegionOptions, bool, IHandler, Promise>(AddView),
                    fromView, view, options, removeFromView, composer);

            if (_unwinding || Children.Contains(view))
                return Promise.Empty;

            var activeView = ActiveView;
            var animation  = options?.Animation;

            if (animation != null)
            {
                var animator = composer.BestEffort().Resolve()
                    .Proxy<IMapping>().Map<IAnimator>(animation);
                if (animator != null)
                    return animator.Present(fromView, view, removeFromView);
            }

            var fromIndex = Children.IndexOf(fromView);

            if (fromIndex >= 0)
                Children.Insert(fromIndex + 1, view);
            else
                Children.Add(view);

            if (fromIndex < 0 || ReferenceEquals(fromView, activeView))
                view.Focus();

            if (removeFromView && fromView != null)
                RemoveView(fromView, null,  null, composer);

            return Promise.Empty;
        }

        private Promise RemoveView(ViewController fromView,
            ViewController toView, IAnimation animation,
            IHandler composer)
        {
            if (!Dispatcher.CheckAccess())
                return (Promise)Dispatcher.Invoke(
                    new Func<ViewController, ViewController,
                    IAnimation, IHandler, Promise>(RemoveView)
                    , fromView, composer);

            if (animation != null)
            {
                var animator = composer.BestEffort().Resolve()
                    .Proxy<IMapping>().Map<IAnimator>(animation);
                if (animator != null)
                    return animator.Dismiss(fromView, toView);
            }

            fromView.RemoveView();
            return Promise.Empty;
        }

        private static RegionOptions GetRegionOptions(IHandler composer)
        {
            if (composer == null) return null;
            var options = new RegionOptions();
            return composer.Handle(options, true) ? options : null;
        }

        #endregion

        #region ViewLayer

        public class ViewLayer : IViewLayer
        {
            private readonly bool _overlay;
            private ViewController _view;
            private IAnimation _animation;
            private IHandler _composer;
            protected bool _disposed;

            public ViewLayer(ViewRegion region, bool overlay)
            {
                _overlay = overlay;
                Events   = new EventHandlerList();
                Region   = region;
            }

            protected ViewRegion       Region { get; }
            protected EventHandlerList Events { get; }

            public ViewController View
            {
                get => _view;
                private set
                {
                    var view = (IView)_view?.Content;
                    if (Region.DoesDependOn(view))
                        view.Release();
                    _view = value;
                    if (_view != null)
                    {
                        view = (IView)_view.Content;
                        if (view.Policy.Parent == null)
                            Region.DependsOn(view);
                    }
                }
            }

            public int Index => Region.GetLayerIndex(this);

            public event EventHandler Transitioned
            {
                add => Events.AddHandler(TransitionedEvent, value);
                remove => Events.RemoveHandler(TransitionedEvent, value);
            } protected static readonly object TransitionedEvent = new object();

            public event EventHandler Disposed
            {
                add
                {
                    if (_disposed)
                        value(this, EventArgs.Empty);
                    else
                        Events.AddHandler(DisposedEvent, value);
                }
                remove => Events.RemoveHandler(DisposedEvent, value);
            } protected static readonly object DisposedEvent = new object();

            public IViewLayer TransitionTo(IView view,
                RegionOptions options, IHandler composer)
            {
                _composer = composer;

                if (ReferenceEquals(View?.Content, view)) 
                    return this;

                // The initial animation will be captured
                // and used when the layer is transitioned from

                if (_animation == null)
                    _animation = options?.Animation;

                var oldView = View;
                if (_overlay && oldView != null)
                {
                    var layer = Region.DropLayer(this);
                    if (layer != null)
                    {
                        var actual = layer.TransitionTo(view, options, composer);
                        Region.RemoveView(oldView, null, null, composer);
                        return actual;
                    }
                }

                var removeFromView = oldView != null;
                if (!removeFromView)
                {
                    var below = Region.GetLayerBelow(this);
                    if (below != null)
                        oldView = below.View;
                }
                View = new ViewController(Region, view);
                Region.AddView(oldView, View, options, removeFromView, composer);
                Events.Raise(this, TransitionedEvent);

                return this;
            }

            public void TransitionFrom()
            {
                var dispatcher = Region.Dispatcher;
                if (!dispatcher.CheckAccess())
                {
                    if (!dispatcher.HasShutdownStarted && 
                        !dispatcher.HasShutdownFinished)
                        dispatcher.Invoke(TransitionFrom);
                    return;
                }
                var oldView    = View;
                var activeView = Region.ActiveView;
                if (oldView != null && 
                    !ReferenceEquals(oldView, activeView))
                    Region.RemoveView(oldView, activeView, _animation, _composer);
                View = null;
            }

            public IDisposable Duration(TimeSpan duration, Action<bool> complete)
            {
                if (!Region.Dispatcher.CheckAccess())
                    return Region.Dispatcher.Invoke(() => Duration(duration, complete));

                DispatcherTimer timer = null;

                void StopTimer(bool cancelled, Action<bool> c)
                {
                    var t = timer;
                    if (t == null) return;
                    timer = null;
                    t.IsEnabled = false;
                    c?.Invoke(cancelled);
                }

                void DidTransition(object sender, EventArgs args)
                {
                    StopTimer(true, null);
                    Transitioned -= DidTransition;
                    Disposed     -= DidDispose;
                }
                Transitioned += DidTransition;

                void DidDispose(object sender, EventArgs args)
                {
                    StopTimer(false, null);
                    Disposed     -= DidDispose;
                    Transitioned -= DidTransition;

                }
                Disposed += DidDispose;

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(duration.TotalMilliseconds)
                };
                timer.Tick += (_, e) => StopTimer(false, complete);
                timer.IsEnabled = true;

                return new DisposableAction(() => StopTimer(true, complete));
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed || !disposing) return;
                try
                {
                    Region.RemoveLayer(this);
                }
                finally
                {
                    _disposed = true;
                    Events.Raise(this, DisposedEvent);
                    Events.Dispose();
                }
            }

            ~ViewLayer()
            {
                Dispose(false);
            }
        }

        #endregion
    }
}