namespace Miruken.Mvc.Tests
{
    using Views;

    public class TestView : IView
    {
        public object ViewModel { get; set; }

        public IViewLayer Display(IViewRegion region) => region.Show(this);
    }
}
