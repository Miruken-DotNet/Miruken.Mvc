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

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Roll, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Roll.Fade,
                        fromView, toView, duration, true, Roll.Mode);
                    ZoomAnimator.Apply(storyboard, Roll.Zoom,
                        fromView, toView, duration, true, Roll.Mode);
                    Apply(storyboard, Roll, fromView, toView, duration);
                }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Roll, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Roll.Fade,
                        fromView, toView, duration, false, Roll.Mode);
                    ZoomAnimator.Apply(storyboard, Roll.Zoom,
                        fromView, toView, duration, false, Roll.Mode);
                    Apply(storyboard, Roll, fromView, toView, duration, false);
                });
        }

        public static void Apply(TimelineGroup storyboard,
            Roll roll, ViewController fromView, ViewController toView,
            TimeSpan duration, bool present = true, 
            Mode? defaultMode = null)
        {
            if (roll == null) return;
            switch (roll.Mode ?? defaultMode ?? Mode.Out)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, roll, toView, false, duration);
                    }
                    else
                        Apply(storyboard, roll, fromView, true, duration);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, roll, fromView, true, duration);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, roll, toView, false, duration);
                    }
                    break;
                case Mode.InOut:
                    if (present)
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

            AddRotation(storyboard, view, anchor, rollOut, duration,
                roll.Behaviors.Find<IEasingFunction>());

            if (roll.Zoom != null)
                AddSkew(storyboard, view, anchor, rollOut, duration);
        }

        private static void AddRotation(TimelineGroup storyboard,
            ViewController view, Position anchor, bool rollOut, 
            TimeSpan duration, IEasingFunction ease)
        {
            var rotation = new DoubleAnimation
            {
                Duration       = duration,
                EasingFunction = ease
            };
            switch (anchor)
            {
                case Position.TopRight:
                case Position.BottomLeft:
                case Position.MiddleLeft:
                    rotation.From = rollOut ? 0 : -90;
                    rotation.To   = rollOut ? -90 : 0;
                    break;
                case Position.TopLeft:
                case Position.BottomRight:
                case Position.MiddleRight:
                    rotation.From = rollOut ? 0 : 90;
                    rotation.To   = rollOut ? 90 : 0;
                    break;
                case Position.TopCenter:
                case Position.MiddleCenter:
                    rotation.From = rollOut ? 0 : -180;
                    rotation.To   = rollOut ? -180 : 0;
                    break;
                case Position.BottomCenter:
                    rotation.From = rollOut ? 0 : 180;
                    rotation.To   = rollOut ? 180 : 0;
                    break;
                default:
                    throw new InvalidOperationException("Invalid anchor point");
            }
            var property = view.AddTransform(new RotateTransform());
            storyboard.Children.Add(rotation);
            Storyboard.SetTarget(rotation, view);
            Storyboard.SetTargetProperty(rotation,
                property(RotateTransform.AngleProperty));
        }

        private static void AddSkew(TimelineGroup storyboard,
            ViewController view, Position anchor, bool rollOut,
            TimeSpan duration)
        {
            DoubleAnimation skewX;
            DoubleAnimation skewY;

            switch (anchor)
            {
                case Position.TopLeft:
                case Position.TopCenter:
                case Position.BottomRight:
                    skewX = skewY = rollOut
                          ? new DoubleAnimation(0, 45, duration)
                          : new DoubleAnimation(45, 0, duration);
                    break;
                case Position.TopRight:
                case Position.BottomLeft:
                case Position.BottomCenter:
                    skewX = skewY = rollOut
                          ? new DoubleAnimation(0, -45, duration)
                          : new DoubleAnimation(-45, 0, duration);
                    break;
                case Position.MiddleLeft:
                    skewX = rollOut
                          ? new DoubleAnimation(0, -45, duration)
                          : new DoubleAnimation(-45, 0, duration);
                    skewY = rollOut
                          ? new DoubleAnimation(0, 45, duration)
                          : new DoubleAnimation(45, 0, duration);
                    break;
                case Position.MiddleRight:
                    skewX = rollOut
                          ? new DoubleAnimation(0, 45, duration)
                          : new DoubleAnimation(45, 0, duration);
                    skewY = rollOut
                          ? new DoubleAnimation(0, -45, duration)
                          : new DoubleAnimation(-45, 0, duration);
                    break;
                default:
                    return;
            }

            var property = view.AddTransform(new SkewTransform());

            storyboard.Children.Add(skewX);
            Storyboard.SetTarget(skewX, view);
            Storyboard.SetTargetProperty(skewX,
                property(SkewTransform.AngleXProperty));

            storyboard.Children.Add(skewY);
            Storyboard.SetTarget(skewY, view);
            Storyboard.SetTargetProperty(skewY,
                property(SkewTransform.AngleYProperty));
        }
    }
}
