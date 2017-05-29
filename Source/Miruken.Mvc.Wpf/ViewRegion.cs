namespace Miruken.Mvc.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Callback;
    using Context;
    using Infrastructure;
    using Options;
    using Views;

    public class ViewRegion : ViewContainer, IViewStackView
    {
        private bool _unwinding;

        public ViewRegion()
        {
            Layers = new List<ElementLayer>();
            Policy.OnRelease(UnwindLayers);
        }

        private List<ElementLayer> Layers { get; }

        private FrameworkElement ActiveElement
        {
            get
            {
                var activeLayer = ActiveLayer;
                return activeLayer?.Element;
            }
        }

        protected override IViewLayer Show(IView view, IHandler composer)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var element = view as FrameworkElement;
            return element == null
                 ? Handler.Unhandled<IViewLayer>()
                 : TransitionTo(element, composer);
        }

        private IViewLayer TransitionTo(FrameworkElement element, IHandler composer)
        {
            var options       = GetRegionOptions(composer);
            var windowOptions = options?.Window;
            if (windowOptions != null)
                return CreateWindow(windowOptions, (IView)element, composer);

            var          push    = false;
            var          overlay = false;
            ElementLayer layer   = null;

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
                    layer = (ElementLayer) layerOptions.Choose(
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
            return layer.TransitionTo(element, options);
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
                handler = region.ToHandler().Chain(composer);
            var layer = handler.SuppressWindows().P<IViewRegion>().Show(content);
            layer.Disposed += (s, e) => window.Close();
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

        protected override Size ArrangeOverride(Size finalsize)
        {
            ActiveElement?.Arrange(new Rect(new Point(0,0), finalsize));
            return finalsize;
        }

        #region Layer Methods

        private ElementLayer ActiveLayer => 
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

        private ElementLayer DropLayer(ElementLayer layer)
        {
            var index = Layers.IndexOf(layer);
            if (index <= 0) return null;
            Layers.RemoveAt(index);
            return Layers[index - 1];
        }

        private void RemoveLayer(ElementLayer layer)
        {
            Layers.Remove(layer);
            layer.TransitionFrom();
        }

        private ElementLayer CreateLayer(bool overlay)
        {
            var layer = new ElementLayer(this, overlay);
            Layers.Add(layer);
            return layer;
        }

        private int GetLayerIndex(ElementLayer layer)
        {
            return Layers.IndexOf(layer);
        }

        #endregion

        #region Helper Methods

        private void AddElement(
            FrameworkElement fromElement, FrameworkElement element, RegionOptions options)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(
                    new Action<FrameworkElement, FrameworkElement, RegionOptions>(AddElement),
                    fromElement, element, options);
                return;
            }

            if (_unwinding || Children.Contains(element)) return;

            var activeelement = ActiveElement;
            var fromIndex     = Children.IndexOf(fromElement);

            element.Visibility = Visibility.Hidden;

            if (fromIndex >= 0)
                Children.Insert(fromIndex + 1, element);
            else
                Children.Add(element);

            element.Visibility = Visibility.Visible;

            if (fromIndex >= 0 && !ReferenceEquals(fromElement, activeelement))
                return;

            element.Focus();
        }

        private void RemoveElement(FrameworkElement element)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action<FrameworkElement>(RemoveElement), element);
                return;
            }

            if (Children.Contains(element))
                Children.Remove(element);
        }

        private static RegionOptions GetRegionOptions(IHandler composer)
        {
            if (composer == null) return null;
            var options = new RegionOptions();
            return composer.Handle(options, true) ? options : null;
        }

        #endregion

        #region ElementLayer

        public class ElementLayer : IViewLayer
        {
            private readonly bool _overlay;
            private FrameworkElement _element;
            protected bool _disposed;

            public ElementLayer(ViewRegion region, bool overlay)
            {
                _overlay = overlay;
                Events   = new EventHandlerList();
                Region   = region;
            }

            protected ViewRegion       Region { get; }
            protected EventHandlerList Events { get; }

            public FrameworkElement Element
            {
                get { return _element; }
                set
                {
                    if (ReferenceEquals(_element, value))
                        return;
                    var view = (IView)_element;
                    if (Region.DoesDependOn(view))
                        view.Release();
                    _element = value;
                    if (_element != null)
                    {
                        var elementView = (IView)_element;
                        if (elementView.Policy.Parent == null)
                            Region.DependsOn(elementView);
                    }
                }
            }

            public int Index => Region.GetLayerIndex(this);

            public event EventHandler Transitioned
            {
                add { Events.AddHandler(TransitionedEvent, value); }
                remove { Events.RemoveHandler(TransitionedEvent, value); }
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
                remove { Events.RemoveHandler(DisposedEvent, value); }
            } protected static readonly object DisposedEvent = new object();

            public IViewLayer TransitionTo(FrameworkElement element, RegionOptions options)
            {
                if (ReferenceEquals(Element, element))
                    return this;

                // The initial animation will be captured
                // and used when the layer is transitioned from

                var oldElement = Element;
                if (_overlay && oldElement != null)
                {
                    var layer = Region.DropLayer(this);
                    if (layer != null)
                    {
                        var actual = layer.TransitionTo(element, options);
                        Region.RemoveElement(oldElement);
                        return actual;
                    }
                }

                Region.AddElement(Element, element, options);
                Element = element;
                if (oldElement != null)
                    Region.RemoveElement(oldElement);

                Events.Raise(this, TransitionedEvent);
                return this;
            }

            public void TransitionFrom()
            {
                var oldElement = Element;
                if ((oldElement != null) && !ReferenceEquals(oldElement, Region.ActiveElement))
                    Region.RemoveElement(oldElement);
                Element = null;
            }

            public IDisposable Duration(TimeSpan duration, Action<bool> complete)
            {
                if (!Region.Dispatcher.CheckAccess())
                    return Region.Dispatcher.Invoke(() => Duration(duration, complete));

                DispatcherTimer timer = null;
                Action<bool, Action<bool>> stopTimer = (cancelled, c) =>
                {
                    var t = timer;
                    if (t == null) return;
                    timer = null;
                    t.IsEnabled = false;
                    c?.Invoke(cancelled);
                };

                EventHandler transitioned = null;
                EventHandler disposed = null;

                transitioned = (s, a) =>
                {
                    stopTimer(true, null);
                    Transitioned -= transitioned;
                    Disposed -= disposed;
                };
                Transitioned += transitioned;

                disposed = (s, a) =>
                {
                    stopTimer(false, null);
                    Disposed -= disposed;
                    Transitioned -= transitioned;
                };
                Disposed += disposed;

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(duration.TotalMilliseconds)
                };
                timer.Tick += (_, e) => stopTimer(false, complete);
                timer.IsEnabled = true;

                return new DisposableAction(() => stopTimer(true, complete));
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

            ~ElementLayer()
            {
                Dispose(false);
            }
        }

        #endregion
    }
}