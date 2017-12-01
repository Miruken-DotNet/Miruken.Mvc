namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class RollAnimator : Animator
    {
        public RollAnimator(Roll roll)
        {
            if (roll == null)
                throw new ArgumentNullException(nameof(roll));
            Roll = roll;
        }

        public Roll Roll { get; }

        public override Promise Animate(
            ViewController fromView, ViewController toView)
        {
            return Animate(Roll, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Roll.Fade,
                        fromView, toView, duration, Roll.Mode);
                    ZoomAnimator.Apply(storyboard, Roll.Zoom,
                        fromView, toView, duration, Roll.Mode);
                    Apply(storyboard, Roll, fromView, toView, duration);
                });
        }

        public static void Apply(TimelineGroup storyboard,
            Roll roll, ViewController fromView, ViewController toView,
            TimeSpan duration, Mode? defaultMode = null)
        {
            if (roll == null) return;
            switch (roll.Mode ?? defaultMode ?? Mode.Out)
            {
                case Mode.In:
                    toView.AddViewAbove(fromView);
                    Apply(storyboard, roll, toView, false, duration);
                    break;
                case Mode.Out:
                    toView?.AddViewBelow(fromView);
                    Apply(storyboard, roll, fromView, true, duration);
                    break;
                case Mode.InOut:
                    toView.AddViewAbove(fromView);
                    Apply(storyboard, roll, toView, false, duration);
                    Apply(storyboard, roll, fromView, true, duration);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Roll roll, ViewController view, bool rollOut,
            TimeSpan duration)
        {
            if (roll == null) return;

            var anchor = roll.Anchor ?? Position.BottomLeft;
            view.RenderTransformOrigin = ConvertToPoint(anchor);
            var property = view.AddTransform(new RotateTransform());

            var animation = new DoubleAnimation
            {
                Duration       = duration,
                EasingFunction = roll.Behaviors.Find<IEasingFunction>()
            };
            Configure(animation, anchor, rollOut);

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation,
                property(RotateTransform.AngleProperty));
        }

        private static void Configure(
            DoubleAnimation animation, Position position, bool rollOut)
        {
            switch (position)
            {
                case Position.TopLeft:
                case Position.BottomLeft:
                case Position.MiddleLeft:
                    animation.From = rollOut ? 0 : -90;
                    animation.To   = rollOut ? -90 : 0;
                    break;
                case Position.TopRight:
                case Position.BottomRight:
                case Position.MiddleRight:
                    animation.From = rollOut ? 0 : 90;
                    animation.To   = rollOut ? 90 : 0;
                    break;
                case Position.TopCenter:
                case Position.MiddleCenter:
                    animation.From = rollOut ? 0 : -180;
                    animation.To   = rollOut ? -180 : 0;
                    break;
                case Position.BottomCenter:
                    animation.From = rollOut ? 0 : 180;
                    animation.To   = rollOut ? 180 : 0;
                    break;
                default:
                    throw new InvalidOperationException("Invalid position");
            }
        }
    }
}
