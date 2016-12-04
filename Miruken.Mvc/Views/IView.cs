using System;
using SixFlags.CF.Miruken.MVC.Policy;

namespace SixFlags.CF.Miruken.MVC.Views
{
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

        int Index { get; }

        IDisposable Duration(TimeSpan duration, Action<bool> complete);
    }
}