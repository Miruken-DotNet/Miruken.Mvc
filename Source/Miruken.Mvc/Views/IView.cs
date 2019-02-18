using System;

namespace Miruken.Mvc.Views
{
    using Concurrency;

    public interface IView
    {
        object ViewModel { get; set; }

        IViewLayer Display(IViewRegion region);
    }

    public interface IViewLayer : IDisposable
    {
        event EventHandler Transitioned;
        event EventHandler Disposed;

        int Index { get; }

        Promise Duration(TimeSpan duration);
    }
}