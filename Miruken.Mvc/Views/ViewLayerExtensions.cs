namespace Miruken.Mvc.Views
{
    using System;

    public static class ViewLayerExtensions
    {
        public static IDisposable Duration(
            this IView view, TimeSpan duration, Action<bool> complete)
        {
            return view.Layer.Duration(duration, complete);
        }
    }
}
