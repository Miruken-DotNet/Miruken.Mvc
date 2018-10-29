using System;

namespace Miruken.Mvc.Views
{
    public interface IViewRegion
    {
        IViewStackView CreateViewStack();

        V View<V>(Action<V> init = null) where V : IView;

        IViewLayer Show<V>(Action<V> init = null) where V : IView;

        IViewLayer Show(IView view);
    }
}
