using System;

namespace Miruken.Mvc
{
    public enum NavigationStyle
    {
        Next,
        Push
    }

    public interface INavigate
    {
        object Next<C>(
            Func<C, object> action) 
            where C : class, IController;

        object Push<C>(
            Func<C, object> action) 
            where C : class, IController;

        object Navigate<C>(
            Func<C, object> action, NavigationStyle style)
            where C : class, IController;

        object GoBack();
    }
}
