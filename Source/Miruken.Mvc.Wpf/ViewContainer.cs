namespace Miruken.Mvc.Wpf
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using Callback;
    using Views;

    public abstract class ViewContainer : Grid, IViewRegion, IView
    {
        [EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ViewModel
        {
            get => DataContext;
            set => DataContext = value;
        }

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
            if (!Dispatcher.CheckAccess())
            {
                return (V)Dispatcher.Invoke(
                    new Func<Action<V>, Handler, V>(View),
                    init, composer);
            }

            var viewType = typeof(V);
            if (viewType.IsInterface || viewType.IsAbstract)
                return Handler.Unhandled<V>();

            var view = Activator.CreateInstance<V>();
            init?.Invoke(view);
            return view;
        }

        public IViewLayer Show<V>(Action<V> init = null) where V : IView
        {
            var composer = HandleMethod.RequireComposer();
            return Dispatcher.CheckAccess()
                 ? Show(View(init, composer), composer)
                 : Dispatcher.Invoke(() => 
                    Show(View(init, composer), composer));
        }

        public IViewLayer Show(IView view)
        {
            return Show(view, HandleMethod.RequireComposer());
        }

        public abstract IViewStackView CreateViewStack();

        protected abstract IViewLayer Show(IView view, IHandler composer);
    }
}
