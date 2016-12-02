namespace Miruken.Mvc.Views
{
    public abstract class ViewAdapter : IView
    {
        protected ViewAdapter()
        {
            Policy = new ViewPolicy(this);
        }

        public IController Controller { get; set; }

        public ViewPolicy  Policy     { get; set; }

        public IViewLayer  Layer      { get; protected set; }

        public abstract IViewLayer Display(IViewRegion region);
        
    }

    public class RegionAdapter : IViewRegion
    {
        public RegionAdapter(IViewRegion inner)
        {
            Inner = inner;
        }

        public IViewRegion Inner     { get; private set; }

        public IViewLayer  ViewLayer { get; private set; }

        public virtual IViewLayer Show(IView view)
        {
            return (ViewLayer = view.Display(Inner));
        }
    }
}
