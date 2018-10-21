using System;

namespace Miruken.Mvc.Views
{
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

        IDisposable Duration(TimeSpan duration, Action<bool> complete);
    }
}