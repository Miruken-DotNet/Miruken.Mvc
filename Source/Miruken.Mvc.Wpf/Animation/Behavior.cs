namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows.Media.Animation;
    using Callback;
    using Mvc.Animation;
    using Options;

    public class Behavior : Animation
    {
        public override IAnimation Merge(IAnimation other)
        {
            return other is Behavior 
                 ? base.Merge(other)
                 : other.Merge(this);
        }
    }

    #region BehaviorExtensions

    public static class BehaviorExtensions
    {
        public static IHandler Behavior(
            this IHandler handler, object behavior)
        {
            return new RegionOptions
            {
                Animation = new Behavior
                {
                    Behaviors = { behavior }
                }
            }.Decorate(handler);
        }

        public static IHandler Accelerate(
            this IHandler handler, double acceleration)
        {
            return handler.Behavior(
                new TimelineBehavior { Acceleration = acceleration });
        }

        public static IHandler Speed(
            this IHandler handler, double speed)
        {
            return handler.Behavior(
                new TimelineBehavior { Speed = speed });
        }

        public static IHandler Ease(
            this IHandler handler, IEasingFunction easing)
        {
            return handler.Behavior(easing);
        }

        public static IHandler Timeline(
            this IHandler handler, TimelineBehavior timeline)
        {
            return handler.Behavior(timeline);
        }
    }

    #endregion
}
