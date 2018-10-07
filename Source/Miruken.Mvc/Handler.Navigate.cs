namespace Miruken.Mvc
{
    using System;
    using Callback;
    using Options;

    public static class HandlerNavigateExtensions
    {
        public static object Next<C>(
            this IHandler handler, Func<C, object> action)
            where C : IController
        {
            return Navigate(handler, action, NavigationStyle.Next);
        }

        public static C Next<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Next);
        }

        public static object Push<C>(
            this IHandler handler, Func<C, object> action)
            where C : IController
        {
            return Navigate(handler, action, NavigationStyle.Push);
        }

        public static C Push<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Push);
        }

        public static object Navigate<C>(
            this IHandler handler, Func<C, object> action,
            NavigationStyle style)
            where C : IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var navigation = new Navigation(typeof(C),
                ctrl => action((C)ctrl), style);

            if (!handler.Handle(navigation))
                throw new NotSupportedException(
                    $"Navigation to {navigation.ControllerType} not handled");
            return navigation.ClearResult();
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            if (style == NavigationStyle.Push)
                handler = handler.PushLayer();
            return (C)new NavigateInterceptor<C>(handler, style)
                .GetTransparentProxy();
        }

        public static object GoBack(this IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var goBack = new GoBack();
            if (!handler.Handle(goBack))
                throw new NotSupportedException(
                    "Navigation backwards not handled");
            return goBack.ClearResult();
        }
    }
}