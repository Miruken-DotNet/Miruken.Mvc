namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Split : Animation
    {
        public Origin? Start { get; set; }
        public int?    Width { get; set; }
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
            this IHandler handler, Mode? mode = null,
            Origin? start = null, TimeSpan? duration = null)
        {
            return handler.Split(new Split
            {
                Mode     = mode,
                Start    = start,
                Duration = duration
            });
        }

        public static IHandler SplitIn(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null)
        {
            return handler.Split(Mode.In, start, duration);
        }

        public static IHandler SplitOut(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null)
        {
            return handler.Split(Mode.Out, start, duration);
        }

        public static IHandler SplitInOut(
            this IHandler handler, Origin? start = null,
            TimeSpan? duration = null)
        {
            return handler.Split(Mode.InOut, start, duration);
        }
    }
}
