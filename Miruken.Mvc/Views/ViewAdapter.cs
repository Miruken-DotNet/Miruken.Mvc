using System;

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

        public IViewRegion Inner     { get; }

        public IViewLayer  ViewLayer { get; private set; }

        public V View<V>() where V : IView
        {
            return Inner.View<V>();
        }

        public V View<V>(Action<V> init) where V : IView
        {
            return Inner.View(init);
        }

        public IViewLayer Show<V>() where V : IView
        {
            return Show(View<V>());
        }

        public IViewLayer Show<V>(Action<V> init) where V : IView
        {
            return Show(View(init));
        }

        public virtual IViewLayer Show(IView view)
        {
            return (ViewLayer = view.Display(Inner));
        }
    }
}
