using System;
using Miruken.Mvc;

namespace Miruken.MVC
{
    public enum NavigationStyle
    {
        Next,
        Push
    }

    public interface INavigate
    {
        object Next<C>(Func<C, object> action) where C : IController;

        object Push<C>(Func<C, object> action) where C : IController;

        object Navigate<C>(Func<C, object> action, NavigationStyle style)
            where C : IController;

        object GoBack();
    }
}
