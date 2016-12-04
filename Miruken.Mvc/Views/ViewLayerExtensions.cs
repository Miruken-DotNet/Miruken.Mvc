using System;

namespace SixFlags.CF.Miruken.MVC.Views
{
    public static class ViewLayerExtensions
    {
        public static IDisposable Duration(
            this IView view, TimeSpan duration, Action<bool> complete)
        {
            return view.Layer.Duration(duration, complete);
        }
    }
}
