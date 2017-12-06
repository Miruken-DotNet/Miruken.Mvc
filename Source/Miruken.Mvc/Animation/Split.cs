namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Split : Animation
    {
        public Orientation? Orientation { get; set; }
        public int?         Width       { get; set; }
    }

    public static class SplitExtensions
    {
        public static IHandler Split(
            this IHandler handler, Split split)
        {
            if (split == null)
                throw new ArgumentNullException(nameof(split));
            return new RegionOptions
            {
                Animation = split
            }.Decorate(handler);
        }

        public static IHandler Split(
            this IHandler handler, Orientation? orientation = null,
            int? width = null, TimeSpan? duration = null)
        {
            return handler.Split(new Split
            {
                Orientation = orientation,
                Width       = width,
                Duration    = duration
            });
        }

        public static IHandler SplitVertical(
            this IHandler handler, int? width = null,
            TimeSpan? duration = null)
        {
            return handler.Split(Orientation.Vertical, width, duration);
        }

        public static IHandler SplitHorizontal(
            this IHandler handler, int? width = null,
            TimeSpan? duration = null)
        {
            return handler.Split(Orientation.Horizontal, width, duration);
        }
    }
}
