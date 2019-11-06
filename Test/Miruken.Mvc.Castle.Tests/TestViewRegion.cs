namespace Miruken.Mvc.Castle.Tests
{
    using System;
    using Views;

    public class TestViewRegion : IViewStackView
    {
        public object ViewModel { get; set; }

        public IViewStackView CreateViewStack()
        {
            return new TestViewRegion();
        }

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

        public IDisposable PushLayer()
        {
            return null;
        }

        public void UnwindLayers()
        {
        }

        public IViewLayer Display(IViewRegion region)
        {
            return null;
        }
    }
}
