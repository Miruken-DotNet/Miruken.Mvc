namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Roll : Animation
    {
        public Origin? Anchor { get; set; }
        public Zoom    Zoom   { get; set; }

        public override IAnimation Merge(IAnimation other)
        {
            Zoom = Zoom ?? other as Zoom;
            return base.Merge(other);
        }
    }

    #region RollExtensions

    public static class RollExtensions
    {
        public static IHandler Roll(
            this IHandler handler, Roll roll)
        {
            if (roll == null)
                throw new ArgumentNullException(nameof(roll));
            return new NavigationOptions
            {
                Animation = roll
            }.Decorate(handler);
        }

        public static IHandler Roll(
            this IHandler handler, Mode? mode = null,
            Origin? anchor = null, TimeSpan? duration = null)
        {
            return handler.Roll(new Roll
            {
                Mode      = mode,
                Anchor    = anchor,
                Duration  = duration
            });
        }

        public static IHandler RollIn(
            this IHandler handler, Origin? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(Mode.In, anchor, duration);
        }

        public static IHandler RollOut(
            this IHandler handler, Origin? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(Mode.Out, anchor, duration);
        }

        public static IHandler RollInOut(
            this IHandler handler, Origin? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(Mode.InOut, anchor, duration);
        }
    }

    #endregion
}
