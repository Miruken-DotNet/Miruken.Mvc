namespace Miruken.Mvc
{
    using System;
    using Callback;

    public static class HandlerNavigateExtensions
    {
        public static C Next<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Next);
        }

        public static C Push<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Push);
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            return (C) new NavigateInterceptor<C>(handler, style)
                .GetTransparentProxy();
        }

        public static object GoBack(this IHandler handler)
        {
            return handler.Proxy<INavigate>().GoBack();
        }
    }
}