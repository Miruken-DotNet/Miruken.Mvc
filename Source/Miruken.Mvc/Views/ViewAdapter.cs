namespace Miruken.Mvc.Views
{
    public abstract class ViewAdapter : IView
    {
        public object      ViewModel { get; set; }
        public IViewLayer  Layer     { get; protected set; }

        public abstract IViewLayer Display(IViewRegion region);
    }
}
