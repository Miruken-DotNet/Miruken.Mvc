namespace Miruken.Mvc
{
    using System;
    using Callback;
    using Options;

    public static class HandlerNavigateExtensions
    {
        public static void Next<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            Navigate(handler, action, NavigationStyle.Next);
        }

        public static C Next<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Next);
        }

        public static void Push<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            Navigate(handler, action, NavigationStyle.Push);
        }

        public static C Push<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Push);
        }

        public static void Partial<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            Navigate(handler, action, NavigationStyle.Partial);
        }

        public static C Partial<C>(this IHandler handler)
            where C : class, IController
        {
            return Navigate<C>(handler, NavigationStyle.Partial);
        }

        public static void Navigate<C>(
            this IHandler handler, Action<C> action,
            NavigationStyle style)
            where C : IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var options = GetRegionOptions(handler);

            var navigation = new Navigation(typeof(C),
                ctrl => action((C)ctrl), style, options);

            if (!handler.Handle(navigation))
                throw new NotSupportedException(
                    $"Navigation to {navigation.ControllerType} not handled");
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (style == NavigationStyle.Push)
                handler = handler.PushLayer();

            return (C)new NavigateInterceptor<C>(handler, style)
                .GetTransparentProxy();
        }

        public static void GoBack(this IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (!handler.Handle(new Navigation.GoBack(GetRegionOptions(handler))))
                throw new NotSupportedException(
                    "Navigation backwards not handled");
        }

        private static RegionOptions GetRegionOptions(IHandler composer)
        {
            if (composer == null) return null;
            var options = new RegionOptions();
            return composer.Handle(options, true) ? options : null;
        }
    }
}