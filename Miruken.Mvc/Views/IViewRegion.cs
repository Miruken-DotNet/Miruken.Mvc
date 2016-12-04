using System;
using System.Runtime.InteropServices;
using SixFlags.CF.Miruken.Callback;

namespace SixFlags.CF.Miruken.MVC.Views
{
    #region Protocol
    [ComImport,
     Guid(Protocol.Guid),
     CoClass(typeof(ViewRegionProtocol))]
    #endregion
    public interface IViewRegion
    {
        V View<V>() where V : IView;

        V View<V>(Action<V> init) where V : IView;

        IViewLayer Show<V>() where V : IView;

        IViewLayer Show<V>(Action<V> init) where V : IView;

        IViewLayer Show(IView view);
    }

    #region ViewRegionProtocol

    public class ViewRegionProtocol : Protocol, IViewRegion
    {
        public ViewRegionProtocol(IProtocolAdapter adapter) : base(adapter)
        {
        }

        V IViewRegion.View<V>()
        {
            return Do((IViewRegion p) => p.View<V>());
        }

        V IViewRegion.View<V>(Action<V> init)
        {
            return Do((IViewRegion p) => p.View(init));
        }

        IViewLayer IViewRegion.Show<V>()
        {
            return Do((IViewRegion p) => p.Show<V>());
        }

        IViewLayer IViewRegion.Show<V>(Action<V> init)
        {
            return Do((IViewRegion p) => p.Show(init));
        }

        IViewLayer IViewRegion.Show(IView view)
        {
            return Do((IViewRegion p) => p.Show(view));
        }
    }

    #endregion
}
