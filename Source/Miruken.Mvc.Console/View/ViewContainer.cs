namespace Miruken.Mvc.Console
{
    using System;
    using Callback;
    using Views;

    public abstract class ViewContainer : StackPanel, IViewRegion, IView
    {
        private ViewPolicy _policy;

        public ViewPolicy Policy
        {
            get => _policy ?? (_policy = new ViewPolicy(this));
            set => _policy = value;
        }

        public object ViewModel { get; set; }

        public virtual IViewLayer Display(IViewRegion region)
        {
            return region?.Show(this);
        }

        public V View<V>() where V : IView
        {
            return View<V>(null, HandleMethod.RequireComposer());
        }

        public V View<V>(Action<V> init) where V : IView
        {
            return View(init, HandleMethod.RequireComposer());
        }

        protected virtual V View<V>(Action<V> init, IHandler composer)
            where V : IView
        {
            V view;
            if (typeof(V).IsInterface)
            {
                var type = GetType();
                if (!typeof(V).IsAssignableFrom(type))
                    return Handler.Unhandled<V>();
                view = (V)Activator.CreateInstance(type);
            }
            else
                view = Activator.CreateInstance<V>();

            init?.Invoke(view);

            var element = view as View;
            if (element != null)
            {
                if (element.ViewModel == null)
                    element.ViewModel = composer.Resolve<IController>();
                var controller = element.ViewModel as IController;
                controller.DependsOn(view);
            }

            view.Policy.Track();
            return view;
        }

        public IViewLayer Show<V>() where V : IView
        {
            var composer = HandleMethod.RequireComposer();
            return Show(View<V>(null, composer), composer);
        }

        public IViewLayer Show<V>(Action<V> init) where V : IView
        {
            var composer = HandleMethod.RequireComposer();
            return Show(View(init, composer), composer);
        }

        public IViewLayer Show(IView view)
        {
            return Show(view, HandleMethod.RequireComposer());
        }

        protected abstract IViewLayer Show(IView view, IHandler composer);
    }
}
