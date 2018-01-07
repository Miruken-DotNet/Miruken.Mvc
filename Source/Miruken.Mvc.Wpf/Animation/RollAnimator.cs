namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class RollAnimator : BlendAnimator<Roll>
    {
        public RollAnimator(Roll roll) : base(roll)
        {
        }

        public override void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMmode = null)
        {
            var zoom = Animation.Zoom;
            if (zoom == null && Animation.Anchor == Origin.MiddleCenter)
                zoom = new Zoom();

            Zoom(storyboard, zoom, fromView, toView,
                present, defaultMmode ?? Mode.Out);

            base.Transition(storyboard, fromView, toView,
                present, defaultMmode);
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
            var roll   = Animation;
            var anchor = roll.Anchor ?? Origin.BottomLeft;
            view.RenderTransformOrigin = ConvertToPoint(anchor);

            AddRotation(storyboard, view, anchor, animateOut, roll, present);

            if (roll.Zoom != null)
                AddSkew(storyboard, view, anchor, animateOut, roll);
        }

        private static void AddRotation(
            TimelineGroup storyboard, ViewController view,
            Origin anchor, bool rollOut, Roll roll, bool present)
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

        private static void AddSkew(
            TimelineGroup storyboard, ViewController view,
            Origin anchor, bool rollOut, Roll roll)
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
