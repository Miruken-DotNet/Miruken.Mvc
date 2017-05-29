using System;
using Miruken.Container;
using Miruken.Mvc.Views;
using static Miruken.Protocol;

namespace MIruken.Mvc.Castle.Tests
{
    public class TestViewRegion : IViewRegion
    {
        V IViewRegion.View<V>(Action<V> init)
        {
            var view = P<IContainer>(Composer).Resolve<V>();
            init?.Invoke(view);
            return view;
        }

        IViewLayer IViewRegion.Show<V>(Action<V> init)
        {
            var view = ((IViewRegion) this).View(init);
            return ((IViewRegion) this).Show(view);
        }

        IViewLayer IViewRegion.Show(IView view)
        {
            return null;
        }
    }
}
