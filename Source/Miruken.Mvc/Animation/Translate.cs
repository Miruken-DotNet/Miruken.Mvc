namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Translate : Animation
    {
        public Origin? Start { get; set; }
    }

    #region TranslateExtensions

    public static class TranslateExtensions
    {
        public static IHandler Translate(
            this IHandler handler, Translate translate)
        {
            if (translate == null)
                throw new ArgumentNullException(nameof(translate));
            return new RegionOptions
            {
                Animation = translate
            }.Decorate(handler);
        }

        public static IHandler Translate(
            this IHandler handler, Mode? mode = null,
            Origin? start = null, TimeSpan? duration = null)
        {
            return handler.Translate(new Translate
            {
                Mode     = mode,
                Start    = start,
                Duration = duration
            });
        }

        public static IHandler SlideIn(this IHandler handler,
            Origin? start = null, TimeSpan? duration = null)
        {
            return handler.Translate(Mode.In, start, duration);
        }

        public static IHandler SlideOut(this IHandler handler,
            Origin? start = null, TimeSpan? duration = null)
        {
            return handler.Translate(Mode.Out, start, duration);
        }

        public static IHandler Push(this IHandler handler,
            Origin? start = null, TimeSpan? duration = null)
        {
            return handler.Translate(Mode.InOut, start, duration);
        }
    }

    #endregion
}
