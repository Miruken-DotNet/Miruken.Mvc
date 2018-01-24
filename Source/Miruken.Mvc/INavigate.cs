using System;

namespace Miruken.Mvc
{
    using Callback;

    public enum NavigationStyle
    {
        Next,
        Push
    }

    public interface INavigate
    {
        object Next<C>(
            Func<C, object> action,
            Func<IHandler, IHandler> configureIO = null) 
            where C : class, IController;

        object Push<C>(
            Func<C, object> action,
            Func<IHandler, IHandler> configureIO = null) 
            where C : class, IController;

        object Navigate<C>(
            Func<C, object> action, NavigationStyle style,
            Func<IHandler, IHandler> configureIO = null)
            where C : class, IController;

        object GoBack();
    }
}
