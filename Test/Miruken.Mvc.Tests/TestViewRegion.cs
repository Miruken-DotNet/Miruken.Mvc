﻿using System;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Tests
{
    using Concurrency;

    public class TestViewRegion : IViewStackView
    {
        public object ViewModel { get; set; }

        public IViewStackView CreateViewStack()
        {
            return new TestViewRegion();
        }

        V IViewRegion.View<V>(Action<V> init)
        {
            var view =  Activator.CreateInstance<V>();
            init?.Invoke(view);
            return view;
        }

        IViewLayer IViewRegion.Show<V>(Action<V> init)
        {
            return new Layer(((IViewRegion)this).View(init));
        }

        IViewLayer IViewRegion.Show(IView view)
        {
            return new Layer(view);
        }

        public IDisposable PushLayer()
        {
            return new Layer(null);
        }

        public void UnwindLayers()
        {
        }

        public IViewLayer Display(IViewRegion region)
        {
            return region.Show(this);
        }

        public class Layer : IViewLayer
        {
            public Layer(IView view)
            {
                View = view;
            }

            public IView View { get; }

            public int Index { get; } = 0;

            public event EventHandler Transitioned;
            public event EventHandler Disposed;

            public Promise Duration(TimeSpan duration)
            {
                return Promise.Empty;
            }

            public void Dispose()
            {
            }
        }
    }
}
