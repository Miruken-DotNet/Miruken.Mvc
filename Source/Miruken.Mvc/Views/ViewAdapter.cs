namespace Miruken.Mvc.Views
{
    public abstract class ViewAdapter : IView
    {
        protected ViewAdapter()
        {
            Policy = new ViewPolicy(this);
        }

        public object      ViewModel { get; set; }
        public ViewPolicy  Policy    { get; set; }
        public IViewLayer  Layer     { get; protected set; }

        public abstract IViewLayer Display(IViewRegion region);
    }
}
