namespace Miruken.Mvc
{
    using System;
    using Callback;
    using Concurrency;
    using Context;

    public static class HandlerNavigateExtensions
    {
        public static Promise<Context> Next<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            return handler.Navigate(NavigationStyle.Next, action);
        }

        public static C Next<C>(this IHandler handler)
            where C : class, IController
        {
            return handler.Navigate<C>(NavigationStyle.Next);
        }

        public static TargetActionBuilder<C, Promise<Context>> NextBlock<C>(
            this IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Next);
        }

        public static Promise<Context> Push<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            return handler.Navigate(NavigationStyle.Push, action);
        }

        public static C Push<C>(this IHandler handler)
            where C : class, IController
        {
            return handler.Navigate<C>(NavigationStyle.Push);
        }

        public static TargetActionBuilder<C, Promise<Context>> PushBlock<C>(
            this IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Push);
        }

        public static Promise<Context> Partial<C>(
            this IHandler handler, Action<C> action)
            where C : IController
        {
            return handler.Navigate(NavigationStyle.Partial, action);
        }

        public static C Partial<C>(this IHandler handler)
            where C : class, IController
        {
            return handler.Navigate<C>(NavigationStyle.Partial);
        }

        public static TargetActionBuilder<C, Promise<Context>> PartialBlock<C>(
            this IHandler handler)
            where C : IController
        {
            return handler.NavigateBlock<C>(NavigationStyle.Partial);
        }

        public static TargetActionBuilder<C, Promise<Context>> NavigateBlock<C>(
            this IHandler handler, NavigationStyle style)
            where C : IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return new TargetActionBuilder<C, Promise<Context>>(action =>
            {
                var navigation = new Navigation<C>(action, style);
                return handler.CommandAsync<Context>(navigation);
            });
        }

        public static Promise<Context> Navigate<C>(
            this IHandler handler, NavigationStyle style, Action<C> action)
            where C : IController
        {
            return handler.NavigateBlock<C>(style).Invoke(action);
        }

        public static C Navigate<C>(this IHandler handler, NavigationStyle style)
            where C : class, IController
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return (C)new NavigateInterceptor<C>(handler, style)
                .GetTransparentProxy();
        }

        public static Promise<Context> GoBack(this IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return handler.CommandAsync<Context>(new Navigation.GoBack());
        }
    }
}