using System;

namespace Miruken.Mvc.Views
{
    public interface IViewStack : IViewRegion
    {
        IDisposable PushLayer();
        void UnwindLayers();
    }

    public interface IViewStackView : IViewStack, IView
    {  
    }
}
