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

    public class ViewRegion : ViewContainer, IViewStackView, IDisposable
    {
        private bool _unwinding;

        public ViewRegion()
        {
            Layers = new List<ViewLayer>();
        }

        void IDisposable.Dispose()
        {
            UnwindLayers();
        }

        private bool IsChild { get; set; }

        private List<ViewLayer> Layers { get; }

        private ViewController ActiveView
        {
            get
            {
                var activeLayer = ActiveLayer;
                return activeLayer?.View;
            }
        }

        public override IViewStackView CreateViewStack()
        {
            return new ViewRegion { IsChild = true };
        }

        protected override IViewLayer Show(IView view, IHandler composer)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return TransitionTo(view, composer, false);
        }

        private IViewLayer TransitionTo(IView view, IHandler composer, bool noWindow)
        {
            var push        = false;
            var overlay     = false;
            ViewLayer layer = null;

            var navigation    = composer.Resolve<Navigation>();
            var options       = composer.GetOptions(new NavigationOptions());
            var regionOptions = options?.Region;
            var region        = regionOptions?.Region;
            if (region != null)
            {
                if (region != Tag)
                    return Handler.Unhandled<IViewLayer>();
                if (navigation != null)
                    navigation.ViewRegion = region;
            }
            else
            {
                var impliedRegion = navigation?.ViewRegion;
                if (impliedRegion != null && impliedRegion != Tag)
                    return Handler.Unhandled<IViewLayer>();
            }

            var windowOptions = options?.Window;
            if (!(noWindow || windowOptions == null))
                return CreateWindow(windowOptions, view, navigation, composer);

            if (regionOptions != null)
            {
                if (regionOptions.Push == true)
                    push = true;
                else if (regionOptions.Overlay == true)
                {
                    push = overlay = true;
                }
                else if (regionOptions.Unload == true)
                {
                    UnwindLayers();
                    push = true;
                }
                else
                {
                    layer = (ViewLayer) regionOptions.Choose(
                        Layers.Cast<IViewLayer>().ToArray());
                }
            }

            if (push)
            {
                layer = overlay ? CreateLayer(true, true) : (ViewLayer)PushLayer();
            }
            else
            {
                if (layer == null && navigation?.ViewLayer is ViewLayer myLayer &&
                    Layers.Contains(myLayer))
                    layer = myLayer;
                else
                    layer = Layers.FirstOrDefault(l => !l.Push)
                        ?? CreateLayer(bottom: Layers.Count > 0);
            }

            if (layer == null) layer = ActiveLayer;
            if (navigation != null && navigation.ViewLayer == null)
                navigation.ViewLayer = layer;
            BindController(view, layer, navigation);
            return layer.TransitionTo(view, options, composer);
        }

        protected virtual IViewLayer CreateWindow(
            WindowOptions options, IView content, Navigation navigation,
            IHandler composer)
        {
            ViewRegion region;

            var owner = Window.GetWindow(this);
            if (owner == null)  // adopt region
            {
                region = this;
            }
            else
            {
                EnsureCompatibleNavigation(navigation);
                region = new ViewRegion();
            }
            var context = navigation.Controller.Context;
            var window = CreateWindow(owner, options, region);
            if (!ReferenceEquals(region, this) && navigation.Style == NavigationStyle.Push)
            {
                context.AddHandlers(region);
            }
            var layer = region.TransitionTo(content, context, true);
            layer.Disposed += (s, e) =>
            {
                if (window.Dispatcher?.CheckAccess() == true)
                    window.Close();
                else
                    window.Dispatcher?.Invoke(new ThreadStart(window.Close));
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

        private static void EnsureCompatibleNavigation(Navigation navigation)
        {
            if (navigation?.Style == NavigationStyle.Next)
                throw new InvalidOperationException(
                    $"{navigation.ControllerKey} is presenting a new Window, but has no matching context.  Did you do a Next instead of a Push?");
        }

        #region Layer Methods

        private ViewLayer ActiveLayer => 
            Layers.Count > 0 ? Layers[Layers.Count - 1]  : null;

        public IDisposable PushLayer()
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

        private ViewLayer CreateLayer(
            bool push = false, bool overlay = false, bool bottom = false)
        {
            var layer = new ViewLayer(this, push, overlay, bottom);
            if (bottom)
                Layers.Insert(0, layer);
            else 
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
            ViewController view, bool bottom, NavigationOptions options,
            bool removeFromView, IHandler composer)
        {
            if (Dispatcher?.CheckAccess() == false)
                return (Promise)Dispatcher.Invoke(
                    new Func<ViewController, ViewController, bool,
                    NavigationOptions, bool, IHandler, Promise>(AddView),
                    fromView, view, bottom, options, removeFromView, composer);

            if (_unwinding || Children.Contains(view))
                return Promise.Empty;

            var activeView = ActiveView;
            var animation  = options?.Animation;

            if (animation != null && animation != NoAnimation.Instance)
            {
                var animator = composer.BestEffort().Map<IAnimator>(animation);
                if (animator != null)
                    return animator.Present(fromView, view, removeFromView);
            }

            var fromIndex = -1;
            if (bottom)
            {
                var layer = Layers.FirstOrDefault(l => l.View != null);
                if (layer != null)
                    fromIndex = Children.IndexOf(layer.View);
            }
            else
            {
                fromIndex = Children.IndexOf(fromView);
                if (fromIndex >= 0) ++fromIndex;
            }

            if (fromIndex >= 0)
                Children.Insert(fromIndex, view);
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
            if (Dispatcher?.CheckAccess() == false)
                return (Promise)Dispatcher.Invoke(
                    new Func<ViewController, ViewController,
                    IAnimation, IHandler, Promise>(RemoveView),
                    fromView, composer);

            if (animation != null && animation != NoAnimation.Instance)
            {
                var animator = composer.BestEffort().Map<IAnimator>(animation);
                if (animator != null)
                    return animator.Dismiss(fromView, toView);
            }

            fromView.RemoveView();
            return Promise.Empty;
        }

        private void BindController(IView view, IViewLayer layer, Navigation navigation)
        {
            if (view.ViewModel == null)
            {
                var controller = navigation?.Controller;
                if (controller != null)
                {
                    view.ViewModel = controller;
                    var context = controller.Context;
                    if (context != null) 
                    {
                        void DisposeLayer(Context ctx, object reason)
                        {
                            // allows ending animation
                            if ((Layers.Count > 1 || !IsChild) && !(reason is Navigation))
                                layer.Dispose();

                            context.ContextEnded -= DisposeLayer;
                        }
                        context.ContextEnded += DisposeLayer;
                    }
                }
            }
        }

        #endregion

        #region ViewLayer

        public class ViewLayer : IViewLayer
        {
            private IAnimation _animation;
            private IHandler _composer;
            protected bool _disposed;

            public ViewLayer(ViewRegion region,
                bool push = false, bool overlay = false, bool bottom = false)
            {
                Region  = region;
                Push    = push;
                Overlay = overlay;
                Bottom  = bottom;
                Events  = new EventHandlerList();
            }

            public bool Push    { get; }
            public bool Overlay { get; }
            public bool Bottom  { get; }

            protected ViewRegion       Region { get; }
            protected EventHandlerList Events { get; }

            public ViewController View { get; set; }

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
                NavigationOptions options, IHandler composer)
            {
                _composer = composer;

                if (ReferenceEquals(View?.Content, view)) 
                    return this;

                // The initial animation will be captured
                // and used when the layer is transitioned from

                if (_animation == null)
                    _animation = options?.Animation;

                var oldView = View;
                if (Overlay && oldView != null)
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
                Region.AddView(oldView, View, Bottom && oldView == null,
                               options, removeFromView, composer);
                Events.Raise(this, TransitionedEvent);

                return this;
            }

            public void TransitionFrom()
            {
                var dispatcher = Region.Dispatcher;
                if (dispatcher?.CheckAccess() == false)
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

            public Promise Duration(TimeSpan duration)
            {
                return new Promise<bool>(ChildCancelMode.Any, (resolve, reject, onCancel) =>
                {
                    DispatcherTimer timer = null;

                    void StopTimer(bool cancelled, bool complete)
                    {
                        var t = timer;
                        if (t == null) return;
                        timer = null;
                        t.IsEnabled = false;
                        if (complete) resolve(cancelled, false);
                    }

                    onCancel(() => StopTimer(true, true));

                    EventHandler transitioned = null;
                    EventHandler disposed = null;

                    transitioned = (s, a) =>
                    {
                        StopTimer(true, false);
                        Transitioned -= transitioned;
                        Disposed -= disposed;
                    };
                    Transitioned += transitioned;

                    disposed = (s, a) =>
                    {
                        StopTimer(false, false);
                        Disposed -= disposed;
                        Transitioned -= transitioned;
                    };
                    Disposed += disposed;

                    timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(duration.TotalMilliseconds)
                    };
                    timer.Tick += (_, e) => StopTimer(false, true);
                    timer.IsEnabled = true;
                });
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
                    _composer = null;
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