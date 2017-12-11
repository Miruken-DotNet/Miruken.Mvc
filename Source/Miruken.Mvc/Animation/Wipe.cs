namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Wipe : Animation
    {
        public Origin? Start  { get; set; }
        public bool?   Rotate { get; set; }
    }

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
            TimeSpan? duration = null)
        {
            return handler.Wipe(new Wipe
            {
                Mode     = mode,
                Start    = start,
                Rotate   = rotate,
                Duration = duration
            });
        }

        public static IHandler WipeIn(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null)
        {
            return handler.Wipe(Mode.In, start, rotate, duration);
        }

        public static IHandler WipeOut(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null)
        {
            return handler.Wipe(Mode.Out, start, rotate, duration);
        }

        public static IHandler WipeInOut(
            this IHandler handler, Origin? start = null,
            bool? rotate = null, TimeSpan? duration = null)
        {
            return handler.Wipe(Mode.InOut, start, rotate, duration);
        }
    }
}
