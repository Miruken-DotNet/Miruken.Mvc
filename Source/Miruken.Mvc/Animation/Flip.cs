namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Flip : Animation
    {
        public double? Angle { get; set; }
    }

    public static class FlipExtensions
    {
        public static IHandler Flip(
            this IHandler handler, Flip flip)
        {
            if (flip == null)
                throw new ArgumentNullException(nameof(flip));
            return new NavigationOptions
            {
                Animation = flip
            }.Decorate(handler);
        }

        public static IHandler Flip(
            this IHandler handler, Mode? mode = null,
            double? angle = null, TimeSpan? duration = null)
        {
            return handler.Flip(new Flip
            {
                Mode     = mode,
                Angle    = angle,
                Duration = duration
            });
        }

        public static IHandler FlipIn(this IHandler handler,
            double? angle = null, TimeSpan? duration = null)
        {
            return handler.Flip(Mode.In, angle, duration);
        }

        public static IHandler FlipOut(this IHandler handler,
            double? angle = null, TimeSpan? duration = null)
        {
            return handler.Flip(Mode.Out, angle, duration);
        }

        public static IHandler FlipInOut(this IHandler handler,
            double? angle = null, TimeSpan? duration = null)
        {
            return handler.Flip(Mode.InOut, angle, duration);
        }
    }
}
