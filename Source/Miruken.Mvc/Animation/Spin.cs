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
            return new RegionOptions
            {
                Animation = spin
            }.Decorate(handler);
        }

        public static IHandler Spin(
            this IHandler handler,TimeSpan? duration = null)
        {
            return handler.Spin(new Spin { Duration = duration });
        }
    }
}
