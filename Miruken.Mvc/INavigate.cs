using System;
using Miruken.Concurrency;
using Miruken.Context;
using Miruken.Mvc;

namespace Miruken.MVC
{
    public enum NavigationStyle
    {
        Next,
        Push,
        Part
    }

    public interface INavigate
    {
        Promise<IContext> Next<C>(Action<C> action) where C : IController;

        Promise<IContext> Push<C>(Action<C> action) where C : IController;

        Promise<IContext> Part<C>(Action<C> action) where C : IController;

        Promise<IContext> Navigate<C>(Action<C> action, NavigationStyle navStyle)
            where C : IController;

        Promise<IContext> GoBack();
    }
}
