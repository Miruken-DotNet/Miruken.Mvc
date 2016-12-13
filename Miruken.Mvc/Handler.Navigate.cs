using Miruken.Callback;

namespace Miruken.Mvc
{
    public static class HandlerNavigateExtensions
    {
        public static C Next<C>(this IHandler handler)
            where C : IController
        {
            return Navigate<C>(handler, NavigationStyle.Next);
        }

        public static C Push<C>(this IHandler handler)
            where C : IController
        {
            return Navigate<C>(handler, NavigationStyle.Push);
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style)
            where C : IController
        {
            if (handler == null) return default(C);
            return (C)new NavigateInterceptor<C>(handler, style)
                .GetTransparentProxy();
        }
    }
}