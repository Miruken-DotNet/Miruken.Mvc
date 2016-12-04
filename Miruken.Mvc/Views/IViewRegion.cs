using System;
using System.Runtime.InteropServices;
using Miruken.Callback;

namespace Miruken.Mvc.Views
{
    #region Protocol
    [ComImport,
     Guid(Protocol.Guid),
     CoClass(typeof(ViewRegionProtocol))]
    #endregion
    public interface IViewRegion
    {
        V View<V>(Action<V> init = null) where V : IView;

        IViewLayer Show<V>(Action<V> init = null) where V : IView;

        IViewLayer Show(IView view);
    }

    #region ViewRegionProtocol

    public class ViewRegionProtocol : Protocol, IViewRegion
    {
        public ViewRegionProtocol(IProtocolAdapter adapter) : base(adapter)
        {
        }

        V IViewRegion.View<V>(Action<V> init)
        {
            return Do((IViewRegion p) => p.View(init));
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
