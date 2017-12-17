namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Wipe : Animation
    {
        public Origin?   Start           { get; set; }
        public TimeSpan? OverlapDuration { get; set; }
        public bool?     Rotate          { get; set; }
    }

    #region WipeExtensions

    public static class WipeExtensions
    {
        public static IHandler Wipe(
            this IHandler handler, Wipe wipe)
        {
            if (wipe == null)
                throw new ArgumentNullException(nameof(Wipe));
            return new RegionOptions
            {
                Animation = wipe
            }.Decorate(handler);
        }

        public static IHandler Wipe(
            this IHandler handler, Mode? mode = null,
            Origin? start = null, bool? rotate = null,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(new Wipe
            {
                Mode            = mode,
                Start           = start,
                Rotate          = rotate,
                Duration        = duration,
                OverlapDuration = overlapDuration
            });
        }

        public static IHandler WipeIn(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.In, start, rotate,
                duration, overlapDuration);
        }

        public static IHandler WipeOut(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.Out, start, rotate,
                duration, overlapDuration);
        }

        public static IHandler WipeInOut(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.InOut, start, rotate, 
                duration, overlapDuration);
        }

        public static IHandler WipeRotateIn(
            this IHandler handler,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.In, null, true,
                duration, overlapDuration);
        }

        public static IHandler WipeRotateOut(
            this IHandler handler,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.Out, null, true,
                duration, overlapDuration);
        }

        public static IHandler WipeRotateInOut(
            this IHandler handler,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.InOut, null, true,
                duration, overlapDuration);
        }
    }

    #endregion
}
