using System;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Tests
{
    public class TestViewRegion : IViewRegion
    {
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

            public IDisposable Duration(TimeSpan duration, Action<bool> complete)
            {
                return null;
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}
