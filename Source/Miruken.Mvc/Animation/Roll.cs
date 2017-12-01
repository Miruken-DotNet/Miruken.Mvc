namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Roll : Animation
    {
        public Mode?     Mode   { get; set; }
        public Position? Anchor { get; set; }
        public Zoom      Zoom   { get; set; }

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
            return new RegionOptions
            {
                Animation = roll
            }.Decorate(handler);
        }

        public static IHandler Roll(
            this IHandler handler, Mode? mode = null,
            Position? anchor = null, TimeSpan? duration = null)
        {
            return handler.Roll(new Roll
            {
                Mode      = mode,
                Anchor    = anchor,
                Duration  = duration
            });
        }

        public static IHandler RollIn(
            this IHandler handler, Position? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(new Roll
            {
                Mode     = Mode.In,
                Anchor   = anchor,
                Duration = duration
            });
        }

        public static IHandler RollOut(
            this IHandler handler, Position? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(new Roll
            {
                Mode     = Mode.Out,
                Anchor   = anchor,
                Duration = duration
            });
        }

        public static IHandler RollInOut(
            this IHandler handler, Position? anchor = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(new Roll
            {
                Mode     = Mode.InOut,
                Anchor   = anchor,
                Duration = duration
            });
        }
    }

    #endregion
}
