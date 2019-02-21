namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Spin : Animation
    {
    }

    public static class SpinExtensions
    {
        public static IHandler Spin(
            this IHandler handler, Spin spin)
        {
            if (spin == null)
                throw new ArgumentNullException(nameof(spin));
            return new NavigationOptions
            {
                Animation = spin
            }.Decorate(handler);
        }

        public static IHandler Spin(
            this IHandler handler, Mode? mode = null,
            TimeSpan? duration = null)
        {
            return handler.Spin(new Spin
            {
                Mode     = mode,
                Duration = duration
            });
        }

        public static IHandler SpinIn(this IHandler handler,
            TimeSpan? duration = null)
        {
            return handler.Spin(Mode.In, duration);
        }

        public static IHandler SpinOut(this IHandler handler,
            TimeSpan? duration = null)
        {
            return handler.Spin(Mode.Out, duration);
        }

        public static IHandler SpinInOut(this IHandler handler,
            TimeSpan? duration = null)
        {
            return handler.Spin(Mode.InOut, duration);
        }
    }
}
