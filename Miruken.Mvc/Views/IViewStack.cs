using System;

namespace SixFlags.CF.Miruken.MVC.Views
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
