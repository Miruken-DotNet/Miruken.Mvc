namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Fade : Animation
    {
        public override IAnimation Merge(IAnimation other)
        {
            return other?.Merge(this) ?? base.Merge(other);
        }
    }

    public static class FadeExtensions
    {
        public static IHandler Fade(
            this IHandler handler, Fade fade)
        {
            if (fade == null)
                throw new ArgumentNullException(nameof(fade));
            return new RegionOptions
            {
                Animation = fade
            }.Decorate(handler);
        }

        public static IHandler Fade(this IHandler handler,
            Mode? mode = null, TimeSpan? duration = null)
        {
            return handler.Fade(new Fade
            {
                Mode     = mode,
                Duration = duration
            });
        }

        public static IHandler FadeIn(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Fade(Mode.In, duration);
        }

        public static IHandler FadeOut(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Fade(Mode.Out, duration);
        }

        public static IHandler FadeInOut(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Fade(Mode.InOut, duration);
        }
    }
}
