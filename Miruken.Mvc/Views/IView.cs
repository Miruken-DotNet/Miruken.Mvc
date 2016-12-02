namespace Miruken.Mvc.Views
{
    using System;
    using Miruken.Mvc.Policy;

    public interface IView : IPolicyOwner<ViewPolicy>
    {
        IController Controller { get; set; }

        IViewLayer  Layer      { get; }

        IViewLayer  Display(IViewRegion region);
    }

    public interface IView<T> : IView where T : IController
    {
        new T Controller { get; set; }
    }

    public interface IViewLayer : IDisposable
    {
        event EventHandler Transitioned;
        event EventHandler Disposed;

        IDisposable Duration(TimeSpan duration, Action<bool> complete);
    }
}