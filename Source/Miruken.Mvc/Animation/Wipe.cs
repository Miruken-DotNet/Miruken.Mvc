namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Wipe : Animation
    {
        public Origin?   Start           { get; set; }
        public TimeSpan? OverlapDuration { get; set; }
    }

    public class WipeRotate : Animation
    {
        public bool? Converge { get; set; }
        public Zoom  Zoom     { get; set; }

        public override IAnimation Merge(IAnimation other)
        {
            Zoom = Zoom ?? other as Zoom;
            return base.Merge(other);
        }
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
            Origin? start = null,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(new Wipe
            {
                Mode            = mode,
                Start           = start,
                Duration        = duration,
                OverlapDuration = overlapDuration
            });
        }

        public static IHandler WipeIn(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.In, start,
                duration, overlapDuration);
        }

        public static IHandler WipeOut(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.Out, start,
                duration, overlapDuration);
        }

        public static IHandler WipeInOut(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null,
            TimeSpan? overlapDuration = null)
        {
            return handler.Wipe(Mode.InOut, start, 
                duration, overlapDuration);
        }

        public static IHandler WipeRotate(
            this IHandler handler, WipeRotate wipe)
        {
            if (wipe == null)
                throw new ArgumentNullException(nameof(Wipe));
            return new RegionOptions
            {
                Animation = wipe
            }.Decorate(handler);
        }

        public static IHandler WipeRotate(
            this IHandler handler,
            Mode? mode = null,
            bool? converge = false,
            TimeSpan? duration = null)
        {
            return handler.WipeRotate(new WipeRotate
            {
                Converge = converge,
                Duration = duration
            });
        }

        public static IHandler WipeConvergeIn(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.WipeRotate(new WipeRotate
            {
                Mode     = Mode.In,
                Converge = true,
                Duration = duration,
            });
        }

        public static IHandler WipeConvergeOut(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.WipeRotate(new WipeRotate
            {
                Mode     = Mode.Out,
                Converge = true,
                Duration = duration,
            });
        }
    }

    #endregion
}
