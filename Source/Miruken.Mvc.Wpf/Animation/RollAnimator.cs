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
            var zoom = Roll.Zoom;
            if (zoom == null && Roll.Anchor == Origin.MiddleCenter)
                zoom = new Zoom();
            return AnimateStory(Roll, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Roll.Fade,
                    fromView, toView, true, Roll.Mode);
                ZoomAnimator.Apply(storyboard, zoom,
                    fromView, toView, true, Roll.Mode);
                Apply(storyboard, Roll, fromView, toView);
            }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            var zoom = Roll.Zoom;
            if (zoom == null && Roll.Anchor == Origin.MiddleCenter)
                zoom = new Zoom();
            return AnimateStory(Roll, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Roll.Fade,
                    fromView, toView, false, Roll.Mode);
                ZoomAnimator.Apply(storyboard, zoom,
                    fromView, toView, false, Roll.Mode);
                Apply(storyboard, Roll, fromView, toView, false);
            });
        }

        public static void Apply(TimelineGroup storyboard,
            Roll roll, ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMode = null)
        {
            if (roll == null) return;
            switch (roll.Mode ?? defaultMode ?? Mode.Out)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, roll, toView, true, false);
                    }
                    else
                        Apply(storyboard, roll, fromView, false, true);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, roll, fromView, true, true);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, roll, toView, false, false);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView?.AddViewAbove(fromView);
                    Apply(storyboard, roll, toView, present, false);
                    Apply(storyboard, roll, fromView, present, true);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Roll roll, ViewController view, bool present, bool rollOut)
        {
            if (roll == null) return;
            var anchor = roll.Anchor ?? Origin.BottomLeft;
            view.RenderTransformOrigin = ConvertToPoint(anchor);

            AddRotation(storyboard, view, anchor, rollOut, roll, present);

            if (roll.Zoom != null)
                AddSkew(storyboard, view, anchor, rollOut, roll);
        }

        private static void AddRotation(TimelineGroup storyboard,
            ViewController view, Origin anchor, bool rollOut, 
            Roll roll, bool present)
        {
            double angle;

            switch (anchor)
            {
                case Origin.TopRight:
                case Origin.BottomLeft:
                    angle = 90;
                    break;
                case Origin.TopLeft:
                case Origin.BottomRight:
                    angle = -90;
                    break;
                case Origin.TopCenter:
                case Origin.MiddleLeft:
                    angle = 180;
                    break;
                case Origin.BottomCenter:
                case Origin.MiddleRight:
                case Origin.MiddleCenter:
                    angle = -180;
                    break;

                default:
                    throw new InvalidOperationException("Invalid anchor point");
            }

            angle = angle * (present ? 1 : -1);

            var rotation = new DoubleAnimation
            {
                From           = rollOut ? 0 : angle,
                To             = rollOut ? -angle : 0,
                Duration       = storyboard.Duration
            };
            Configure(rotation, roll, rollOut);
            var property = view.AddTransform(new RotateTransform());
            storyboard.Children.Add(rotation);
            Storyboard.SetTarget(rotation, view);
            Storyboard.SetTargetProperty(rotation,
                property(RotateTransform.AngleProperty));
        }

        private static void AddSkew(TimelineGroup storyboard,
            ViewController view, Origin anchor, bool rollOut, Roll roll)
        {
            DoubleAnimation skewX;
            DoubleAnimation skewY;
            var duration = storyboard.Duration;

            switch (anchor)
            {
                case Origin.TopLeft:
                case Origin.TopCenter:
                case Origin.BottomRight:
                    skewX = skewY = rollOut
                          ? new DoubleAnimation(0, 45, duration)
                          : new DoubleAnimation(45, 0, duration);
                    break;
                case Origin.TopRight:
                case Origin.BottomLeft:
                case Origin.BottomCenter:
                    skewX = skewY = rollOut
                          ? new DoubleAnimation(0, -45, duration)
                          : new DoubleAnimation(-45, 0, duration);
                    break;
                case Origin.MiddleLeft:
                    skewX = rollOut
                          ? new DoubleAnimation(0, -45, duration)
                          : new DoubleAnimation(-45, 0, duration);
                    skewY = rollOut
                          ? new DoubleAnimation(0, 45, duration)
                          : new DoubleAnimation(45, 0, duration);
                    break;
                case Origin.MiddleRight:
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

            Configure(skewX, roll, rollOut);
            Configure(skewY, roll, rollOut);

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
