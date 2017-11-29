namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public enum RollAnchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class Roll : Animation
    {
        public bool? Clockwise { get; set; }

        public Roll(RollAnchor anchor)
        {
            Anchor = anchor;
        }

        public RollAnchor Anchor { get; }

        public override IAnimation CreateInverse()
        {
            return new Roll(Anchor)
            {
                Clockwise = !(Clockwise ?? false)
            };
        }
    }

    #region RollExtensions

    public static class RollExtensions
    {
        public static IHandler Roll(
            this IHandler handler,
            RollAnchor anchor, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return new RegionOptions
            {
                Animation = new Roll(anchor)
                {
                    Duration = duration,
                    Clockwise = clockwise
                }
            }.Decorate(handler);
        }

        public static IHandler Roll(
            this IHandler handler, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(RollAnchor.BottomLeft, clockwise, duration);
        }

        public static IHandler RollTopLeft(
            this IHandler handler, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(RollAnchor.TopLeft, clockwise, duration);
        }

        public static IHandler RollTopRight(
            this IHandler handler, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(RollAnchor.TopRight, clockwise, duration);
        }

        public static IHandler RollBottomLeft(
            this IHandler handler, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(RollAnchor.BottomLeft, clockwise, duration);
        }

        public static IHandler RollBottomRight(
            this IHandler handler, bool? clockwise = null,
            TimeSpan? duration = null)
        {
            return handler.Roll(RollAnchor.BottomRight, clockwise, duration);
        }
    }

    #endregion
}
