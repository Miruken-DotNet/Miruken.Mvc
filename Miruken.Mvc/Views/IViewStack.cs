namespace Miruken.Mvc.Views
{
    using System;

    public interface IViewStack : IViewRegion
    {
        IDisposable PushLayer();
        void UnwindLayers();
    }

    public interface IViewStackView : IViewStack, IView
    {  
    }
}
