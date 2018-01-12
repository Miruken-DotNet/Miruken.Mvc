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

        protected static Promise AnimateStory(
            IAnimation animation, 
            ViewController fromView, ViewController toView,
            Action<Storyboard> animations,
            bool removeFromView = true)
        {
            if (animations == null)
                throw new ArgumentNullException(nameof(animations));
            var storyboard = new Storyboard
            {
                Duration = GetDuration(animation)
            };
            animations(storyboard);
            return new Promise<object>((resolve, reject) =>
            {
                WhenComplete(storyboard, () =>
                {
                    storyboard.Remove();
                    if (fromView != null)
                    {
                        if (removeFromView)
                            fromView.RemoveView();
                        fromView.RenderTransform       = Transform.Identity;
                        fromView.RenderTransformOrigin = new Point(0, 0);
                        fromView.OpacityMask           = null;
                        fromView.Clip                  = null;
                    }
                    if (toView != null)
                    {
                        if (!removeFromView)
                            toView.AddViewAbove(fromView);
                        toView.RenderTransform       = Transform.Identity;
                        toView.RenderTransformOrigin = new Point(0, 0);
                        toView.OpacityMask           = null;
                        toView.Clip                  = null;
                    }
                    resolve(null, false);
                });
                storyboard.Begin();
            });
        }

        protected static void WhenComplete(Storyboard storyboard, Action action)
        {
            EventHandler completed = null;
            completed = (s, e) =>
            {
                storyboard.Completed -= completed;
                action?.Invoke();
            };
            storyboard.Completed += completed;
        }

        protected void Fade(
            Storyboard storyboard, Fade fade,
            ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMode = null)
        {
            if (fade != null)
            {
                fade.Mode = fade.Mode ?? defaultMode;
                new FadeAnimator(fade).Transition(
                    storyboard, fromView, toView, present);
            }
        }

        protected void Zoom(
            Storyboard storyboard, Zoom zoom,
            ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMode = null)
        {
            if (zoom != null)
            {
                zoom.Mode = zoom.Mode ?? defaultMode;
                new ZoomAnimator(zoom).Transition(
                    storyboard, fromView, toView, present);
            }
        }

        protected static TimeSpan GetDuration(IAnimation animation)
        {
            return animation.Duration.GetValueOrDefault(DefaultDuration);
        }

        protected static void Configure(
            DoubleAnimation timeline, IAnimation animation, bool from)
        {
            Configure((Timeline)timeline, animation, from);
            var easing = animation.Behaviors.Find<IEasingFunction>();
            if (easing != null)
                timeline.EasingFunction = easing;
        }

        protected static void Configure(
            Timeline timeline, IAnimation animation, bool from)
        {
            var timelineBehavior = animation?.Behaviors.Find<TimelineBehavior>();
            if (timelineBehavior == null) return;
            var acceleration = timelineBehavior.Acceleration;
            if (acceleration.HasValue)
            {
                if (from)
                    timeline.AccelerationRatio = acceleration.Value;
                else
                    timeline.DecelerationRatio = acceleration.Value;
            }
            var speed = timelineBehavior.Speed;
            if (speed.HasValue)
                timeline.SpeedRatio = speed.Value;
            var fill = timelineBehavior.Fill;
            if (fill.HasValue)
                timeline.FillBehavior = fill.Value;
            var repeat = timelineBehavior.Repeat;
            if (repeat.HasValue)
                timeline.RepeatBehavior = repeat.Value;
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
