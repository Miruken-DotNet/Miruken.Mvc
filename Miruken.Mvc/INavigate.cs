using System;
using System.Runtime.InteropServices;
using Miruken.Callback;
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

    #region Protocol
    [ComImport,
     Guid(Protocol.Guid),
     CoClass(typeof(NavigateProtocol))]
    #endregion
    public interface INavigate
    {
        Promise<IContext> Next<C>(Action<C> action) where C : IController;

        Promise<IContext> Push<C>(Action<C> action) where C : IController;

        Promise<IContext> Part<C>(Action<C> action) where C : IController;

        Promise<IContext> Navigate<C>(Action<C> action, NavigationStyle navStyle)
            where C : IController;

        Promise<IContext> GoBack();
    }

    #region NavigateProtocol

    public class NavigateProtocol : NullableProtocol, INavigate
    {
        public NavigateProtocol(IProtocolAdapter adapter) : base(adapter)
        {
        }

        Promise<IContext> INavigate.Next<C>(Action<C> action)
        {
            return Do((INavigate p) => p.Next(action));
        }

        Promise<IContext> INavigate.Push<C>(Action<C> action)
        {
            return Do((INavigate p) => p.Push(action));
        }

        Promise<IContext> INavigate.Part<C>(Action<C> action)
        {
            return Do((INavigate p) => p.Part(action));
        }

        Promise<IContext> INavigate.Navigate<C>(Action<C> action, NavigationStyle navStyle)
        {
            return Do((INavigate p) => p.Navigate(action, navStyle));
        }

        Promise<IContext> INavigate.GoBack()
        {
            return Do((INavigate p) => p.GoBack());
        }
    }

    #endregion
}
