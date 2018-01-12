namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Reveal : Animation
    {
        public Origin? Origin { get; set; }
        public Zoom    Zoom   { get; set; }

        public override IAnimation Merge(IAnimation other)
        {
            Zoom = Zoom ?? other as Zoom;
            return base.Merge(other);
        }
    }

    public static class RevealExtensions
    {
        public static IHandler Reveal(
            this IHandler handler, Reveal reveal)
        {
            if (reveal == null)
                throw new ArgumentNullException(nameof(reveal));
            return new RegionOptions
            {
                Animation = reveal
            }.Decorate(handler);
        }

        public static IHandler Reveal(
            this IHandler handler, Mode? mode = null,
            Origin? origin = null, TimeSpan ? duration = null)
        {
            return handler.Reveal(new Reveal
            {
                Mode     = mode,
                Origin   = origin,
                Duration = duration
            });
        }

        public static IHandler RevealIn(this IHandler handler,
            Origin? origin = null, TimeSpan ? duration = null)
        {
            return handler.Reveal(Mode.In, origin, duration);
        }

        public static IHandler RevealOut(this IHandler handler,
            Origin? origin = null, TimeSpan ? duration = null)
        {
            return handler.Reveal(Mode.Out, origin, duration);
        }

        public static IHandler RevealInOut(this IHandler handler,
            Origin? origin = null, TimeSpan ? duration = null)
        {
            return handler.Reveal(Mode.InOut, origin, duration);
        }

        public static IHandler OpenPortal(this IHandler handler,
            Origin? origin = null, TimeSpan? duration = null)
        {
            return handler.Zoom(scale:.8).RevealOut(origin);
        }

        public static IHandler ClosePortal(this IHandler handler,
            Origin? origin = null, TimeSpan? duration = null)
        {
            return handler.Zoom(scale: .8).RevealIn(origin);
        }
    }
}
