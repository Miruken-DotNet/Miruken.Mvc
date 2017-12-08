namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Zoom : Animation
    {
        public Origin? Origin { get; set; }
    }

    #region ZoomExtensions

    public static class ZomeExtensions
    {
        public static IHandler Zoom(
            this IHandler handler, Zoom zoom)
        {
            if (zoom == null)
                throw new ArgumentNullException(nameof(zoom));
            return new RegionOptions
            {
                Animation = zoom
            }.Decorate(handler);
        }

        public static IHandler Zoom(this IHandler handler,
            Mode? mode = null, Origin? origin = null,
            TimeSpan? duration = null)
        {
            return handler.Zoom(new Zoom
            {
                Mode     = mode,
                Origin   = origin,
                Duration = duration
            });
        }

        public static IHandler ZoomIn(this IHandler handler,
            Origin? origin = null, TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.In, origin, duration);
        }

        public static IHandler ZoomOut(this IHandler handler,
            Origin? origin = null, TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.Out, origin, duration);
        }

        public static IHandler ZoomInOut(this IHandler handler,
            Origin? origin = null, TimeSpan? duration = null)
        {
            return handler.Zoom(Mode.InOut, origin, duration);
        }
    }

    #endregion
}
