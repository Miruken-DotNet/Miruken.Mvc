using System;
using Miruken.Mvc.Views;

namespace Miruken.Mvc.Tests
{
    public class TestViewRegion : IViewRegion
    {
        V IViewRegion.View<V>(Action<V> init)
        {
            return default(V);
        }

        IViewLayer IViewRegion.Show<V>(Action<V> init)
        {
            return null;
        }

        IViewLayer IViewRegion.Show(IView view)
        {
            return null;
        }
    }
}
