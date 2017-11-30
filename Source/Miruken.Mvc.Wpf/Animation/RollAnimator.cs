namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
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
            ViewController oldView, ViewController newView)
        {
            var storyboard = new Storyboard();
            var duration   = GetDuration(Roll);
            var clockwise  = Roll.Clockwise;
            var origin     = newView?.RenderTransformOrigin;

            ApplyFade(storyboard, Roll, oldView, newView, duration);

            if (oldView == null)
            {
                if (newView != null)
                {
                    AddAnimation(storyboard, newView, duration, !clockwise);
                    newView.AddView();
                }
            }
            else
            {
                AddAnimation(storyboard, oldView, duration, clockwise);
                newView?.AddViewBefore(oldView);
            }

            return Animate(storyboard, oldView, newView, () =>
            {
                if (newView != null && origin.HasValue)
                    newView.RenderTransformOrigin = origin.Value;
            });
        }

        private void AddAnimation(TimelineGroup storyboard,
            UIElement view, TimeSpan duration, bool clockwise)
        {
            view.RenderTransformOrigin = CreateOrigin();
            view.RenderTransform       = new RotateTransform();

            var animation = new DoubleAnimation
            {
                From           = 0,
                To             = clockwise ? 90 : -90,
                Duration       = duration,
                EasingFunction = Roll.Behaviors.Find<IEasingFunction>()
            };

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, RotateAngle);
        }

        private Point CreateOrigin()
        {
            switch (Roll.Anchor)
            {
                case RollAnchor.TopLeft:
                    return new Point(0, 0);
                case RollAnchor.TopRight:
                    return new Point(1, 0);
                case RollAnchor.BottomLeft:
                    return new Point(0, 1);
                case RollAnchor.BottomRight:
                    return new Point(1, 1);
            }
            throw new InvalidOperationException("Invalid anchor");
        }
    }
}
