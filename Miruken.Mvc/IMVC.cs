namespace Miruken.Mvc
{
    using System;
    using System.Runtime.InteropServices;
    using Callback;
    using Concurrency;
    using Miruken.Context;
    using Miruken.Mvc.Views;

    public enum NavigationStyle
    {
        Next,
        Push,
        Part
    }

    #region Protocol
    [ComImport,
     Guid(Protocol.Guid),
     CoClass(typeof(MVCProtocol))]
    #endregion
    public interface IMVC
    {
        V View<V>() where V : IView;

        V View<V>(Action<V> init) where V : IView;

        IViewLayer ShowView<V>() where V : IView;

        IViewLayer ShowView<V>(V view) where V : IView;

        IViewLayer ShowView<V>(Action<V> init) where V : IView;

        Promise<IContext> Next<C>(Action<C> action) where C : IController;

        Promise<IContext> Push<C>(Action<C> action) where C : IController;

        Promise<IContext> Part<C>(Action<C> action) where C : IController;

        Promise<IContext> Nav<C>(Action<C> action, NavigationStyle navStyle)
            where C : IController;

        Promise<IContext> GoBack();
    }

    #region MVCProtocol

    public class MVCProtocol : NullableProtocol, IMVC
    {
        public MVCProtocol(IProtocolAdapter adapter) : base(adapter)
        {
        }

        V IMVC.View<V>()
        {
            return Do((IMVC p) => p.View<V>());
        }

        V IMVC.View<V>(Action<V> init)
        {
            return Do((IMVC p) => p.View(init));
        }

        IViewLayer IMVC.ShowView<V>()
        {
            return Do((IMVC p) => p.ShowView<V>());
        }

        IViewLayer IMVC.ShowView<V>(V view)
        {
            return Do((IMVC p) => p.ShowView(view));
        }

        IViewLayer IMVC.ShowView<V>(Action<V> init)
        {
            return Do((IMVC p) => p.ShowView(init));
        }

        Promise<IContext> IMVC.Next<C>(Action<C> action)
        {
            return Do((IMVC p) => p.Next(action));
        }

        Promise<IContext> IMVC.Push<C>(Action<C> action)
        {
            return Do((IMVC p) => p.Push(action));
        }

        Promise<IContext> IMVC.Part<C>(Action<C> action)
        {
            return Do((IMVC p) => p.Part(action));
        }

        Promise<IContext> IMVC.Nav<C>(Action<C> action, NavigationStyle navStyle)
        {
            return Do((IMVC p) => p.Nav(action, navStyle));
        }

        Promise<IContext> IMVC.GoBack()
        {
            return Do((IMVC p) => p.GoBack());
        }
    }

    #endregion
}
