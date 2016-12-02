namespace Miruken.Mvc.Views
{
    using System.Runtime.InteropServices;
    using Callback;

    #region Protocol
    [ComImport,
     Guid(Protocol.Guid),
     CoClass(typeof(ViewRegionProtocol))]
    #endregion
    public interface IViewRegion
    {
        IViewLayer Show(IView view);
    }

    #region ViewRegionProtocol

    public class ViewRegionProtocol :  Protocol, IViewRegion
    {
        public ViewRegionProtocol(IProtocolAdapter adapter) : base(adapter)
        {
        }

        IViewLayer IViewRegion.Show(IView view)
        {
            return Do((IViewRegion p) => p.Show(view));
        }
    }

    #endregion
}
