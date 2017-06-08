namespace MIruken.Mvc.Castle.Tests
{
    using System;
    using Miruken.Mvc.Views;

    public class TestViewRegion : IViewRegion
    {
        V IViewRegion.View<V>(Action<V> init)
        {
            throw new NotImplementedException();
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
