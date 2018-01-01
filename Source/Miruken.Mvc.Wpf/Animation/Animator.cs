namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;
    using Point = System.Windows.Point;

    public abstract class Animator : IAnimator
    {
        protected static readonly TimeSpan DefaultDuration =
            TimeSpan.FromMilliseconds(500);

        public abstract Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView);

        public abstract Promise Dismiss(
            ViewController fromView, ViewController toView);

        protected static Promise AnimateStory(IAnimation animation, 
            ViewController fromView, ViewController toView,
            Action<Storyboard> animations,
            bool removeFromView = true)
        {
            if (animations == null)
                throw new ArgumentNullException(nameof(animations));
            var storyboard = CreateStoryboard(animation);
            animations(storyboard);
            return new Promise<object>((resolve, reject) =>
            {
                EventHandler completed = null;
                completed = (s, e) =>
                {
                    storyboard.Completed -= completed;
                    storyboard.Remove();
                    if (fromView != null)
                    {
                        if (removeFromView)
                            fromView.RemoveView();
                        fromView.RenderTransform       = Transform.Identity;
                        fromView.RenderTransformOrigin = new Point(0, 0);
                    }
                    if (toView != null)
                    {
                        if (!removeFromView)
                            toView.AddViewAbove(fromView);
                        toView.RenderTransform       = Transform.Identity;
                        toView.RenderTransformOrigin = new Point(0, 0);
                    }
                    resolve(null, false);
                };
                storyboard.Completed += completed;
                storyboard.Begin();
            });
        }

        protected static void Configure(
            DoubleAnimation timeline, IAnimation animation)
        {
            var easing = animation.Behaviors.Find<IEasingFunction>();
            if (easing != null)
                timeline.EasingFunction = easing;
        }

        protected static TimeSpan GetDuration(IAnimation animation)
        {
            return animation.Duration.GetValueOrDefault(DefaultDuration);
        }

        private static Storyboard CreateStoryboard(IAnimation animation)
        {
            var storyboard = new Storyboard
            {
                Duration = GetDuration(animation)
            };
            var timelineBehavior = animation?.Behaviors.Find<TimelineBehavior>();
            if (timelineBehavior == null) return storyboard;
            var accelerationRatio = timelineBehavior.AccelerationRatio;
            if (accelerationRatio.HasValue)
                storyboard.AccelerationRatio = accelerationRatio.Value;
            var decelerationRatio = timelineBehavior.DecelerationRatio;
            if (decelerationRatio.HasValue)
                storyboard.DecelerationRatio = decelerationRatio.Value;
            var speedRatio = timelineBehavior.SpeedRatio;
            if (speedRatio.HasValue)
                storyboard.SpeedRatio = speedRatio.Value;
            var fillBehavior = timelineBehavior.FillBehavior;
            if (fillBehavior.HasValue)
                storyboard.FillBehavior = fillBehavior.Value;
            var repeatBehavior = timelineBehavior.RepeatBehavior;
            if (repeatBehavior.HasValue)
                storyboard.RepeatBehavior = repeatBehavior.Value;
            return storyboard;
        }

        protected static Point ConvertToPoint(Origin origin)
        {
            switch (origin)
            {
                case Origin.TopLeft:
                    return new Point(0, 0);
                case Origin.TopCenter:
                    return new Point(.5, 0);
                case Origin.TopRight:
                    return new Point(1, 0);
                case Origin.MiddleLeft:
                    return new Point(0, .5);
                case Origin.MiddleCenter:
                    return new Point(.5, .5);
                case Origin.MiddleRight:
                    return new Point(1, .5);
                case Origin.BottomLeft:
                    return new Point(0, 1);
                case Origin.BottomCenter:
                    return new Point(.5, 1);
                case Origin.BottomRight:
                    return new Point(1, 1);
            }
            throw new InvalidOperationException("Invalid position");
        }
    }
}
