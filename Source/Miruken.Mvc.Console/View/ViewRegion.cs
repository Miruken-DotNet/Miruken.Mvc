namespace Miruken.Mvc.Console
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using Callback;
    using Concurrency;
    using Infrastructure;
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

        private List<ViewLayer> Layers { get; }

        private View ActiveView
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

            return !(view is View element)
                 ? Handler.Unhandled<IViewLayer>()
                 : TransitionTo(element, Handler.Composer);
        }

        public override IViewStackView CreateViewStack()
        {
            return new ViewRegion();
        }

        private IViewLayer TransitionTo(View element, IHandler composer)
        {
            var       push    = false;
            var       overlay = false;
            ViewLayer layer   = null;

            var navigation = composer.Resolve<Navigation>();
            var options    = composer.GetOptions<NavigationOptions>();

            var regionOptions = options?.Region;

            if (Layers.Count == 0)
                push = true;
            else if (regionOptions != null)
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
                    layer = (ViewLayer) regionOptions.Choose(
                        Layers.Cast<IViewLayer>().ToArray());
            }
            if (push)
            {
                if (overlay)
                    PushOverlay();
                else
                    PushLayer();
            }
            else if (layer == null && navigation?.ViewLayer is ViewLayer myLayer)
                layer = myLayer;

            if (layer == null) layer = ActiveLayer;
            if (navigation != null && navigation.ViewLayer == null)
                navigation.ViewLayer = layer;
            return layer.TransitionTo(element, options);
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

        private int GetLayerIndex(ViewLayer layer)
        {
            return Layers.IndexOf(layer);
        }

        #endregion

        #region Helper Methods

        private void AddView(View fromView, View view, NavigationOptions options)
        {
            if (_unwinding || Children.Contains(view)) return;
            Add(view);
            view.Initialize();
            Window.ElementLoaded(view);
        }

        private void RemoveView(View view)
        {
            Remove(view);
            var activeView = ActiveView;
            if (activeView == null) return;
            Window.ElementUnloaded(view);
        }

        #endregion

        #region ViewLayer

        public class ViewLayer : IViewLayer
        {
            private readonly bool _overlay;
            protected bool _disposed;

            public ViewLayer(ViewRegion region, bool overlay)
            {
                _overlay = overlay;
                Events   = new EventHandlerList();
                Region   = region;
            }

            protected ViewRegion       Region { get; }
            protected EventHandlerList Events { get; }

            public View Element { get; set; }

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
            }

            protected static readonly object DisposedEvent = new object();

            public IViewLayer TransitionTo(View element, NavigationOptions options)
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
                        Region.RemoveView(oldElement);
                        return actual;
                    }
                }

                Region.AddView(Element, element, options);
                Element = element;
                if (oldElement != null)
                    Region.RemoveView(oldElement);

                Events.Raise(this, TransitionedEvent);
                return this;
            }

            public void TransitionFrom()
            {
                var oldElement = Element;
                if ((oldElement != null) && !ReferenceEquals(oldElement, Region.ActiveView))
                    Region.RemoveView(oldElement);
                Element = null;
            }

            public Promise Duration(TimeSpan duration)
            {
                return new Promise<bool>(ChildCancelMode.Any, (resolve, reject, onCancel) =>
                {
                    Timer timer = null;

                    void StopTimer(bool cancelled, bool complete)
                    {
                        var t = timer;
                        if (t == null) return;
                        timer = null;
                        t.Change(Timeout.Infinite, Timeout.Infinite);
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

                    timer = new Timer(_ => StopTimer(false, true), null,
                        (int)duration.TotalMilliseconds, Timeout.Infinite);
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