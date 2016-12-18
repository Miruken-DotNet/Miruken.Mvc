namespace Miruken.Mvc
{
    using System;
    using Callback;
    using static Protocol;

    public static class HandlerNavigateExtensions
    {
        public static C Next<C>(this IHandler handler, IController initiator = null)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Next, initiator);
        }

        public static C Push<C>(this IHandler handler, IController initiator = null)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Push, initiator);
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style,
                                    IController initiator = null)
            where C : class, IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            return (C) new NavigateInterceptor<C>(handler, style, initiator)
                .GetTransparentProxy();
        }

        public static object GoBack(this IHandler handler)
        {
            return P<INavigate>(handler).GoBack();
        }
    }
}