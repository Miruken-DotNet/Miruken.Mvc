using System;
using Miruken.Mvc.Policy;

namespace Miruken.Mvc.Views
{
    public interface IView : IPolicyOwner<ViewPolicy>
    {
        object ViewModel { get; set; }

        IViewLayer Layer { get; }

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