﻿namespace Miruken.Mvc.Console
{
    using System;
    using Callback;
    using Context;
    using Views;

    public abstract class ViewContainer : StackPanel, IViewRegion, IView
    {
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
            var composer = HandleMethod.RequireComposer();
            return Show(BindView(view, composer), composer);
        }

        protected abstract IViewLayer Show(IView view, IHandler composer);

        public abstract IViewStackView CreateViewStack();

        private static IView BindView(IView view, IHandler composer)
        {
            if (view.ViewModel == null)
            {
                var navigation = composer.Resolve<Navigation>();
                var controller = navigation?.Controller;
                if (controller != null)
                {
                    view.ViewModel = controller;
                    controller.Context?.Dispose(view as IDisposable);
                }
            }
            return view;
        }
    }
}
