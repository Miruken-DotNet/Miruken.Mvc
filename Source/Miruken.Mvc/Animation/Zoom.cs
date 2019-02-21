namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Zoom : Animation
    {
        public Origin? Origin { get; set; }
        public double? Scale  { get; set; }
    }

    #region ZoomExtensions

    public static class ZomeExtensions
    {
        public static IHandler Zoom(
            this IHandler handler, Zoom zoom)
        {
            if (zoom == null)
                throw new ArgumentNullException(nameof(zoom));
            return new NavigationOptions
            {
                Animation = zoom
            }.Decorate(handler);
        }

        public static IHandler Zoom(this IHandler handler,
            Mode? mode = null, Origin? origin = null,
            double? scale = null, TimeSpan? duration = null)
        {
            return handler.Zoom(new Zoom
            {
                Mode     = mode,
                Origin   = origin,
                Scale    = scale,
                Duration = duration
            });
        }

        public static IHandler ZoomIn(this IHandler handler,
            Origin? origin = null, double? scale = null,
            TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.In, origin, scale, duration);
        }

        public static IHandler ZoomOut(this IHandler handler,
            Origin? origin = null, double? scale = null,
            TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.Out, origin, scale, duration);
        }

        public static IHandler ZoomInOut(this IHandler handler,
            Origin? origin = null, double? scale = null,
            TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.InOut, origin, scale, duration);
        }
    }

    #endregion
}
