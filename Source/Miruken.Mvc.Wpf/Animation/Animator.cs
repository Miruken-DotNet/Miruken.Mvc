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
            TimeSpan.FromMilliseconds(400);

        public abstract Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView);

        public abstract Promise Dismiss(
            ViewController fromView, ViewController toView);

        protected static TimeSpan GetDuration(IAnimation animation)
        {
            return animation.Duration.GetValueOrDefault(DefaultDuration);
        }

        protected static Promise AnimateStory(IAnimation animation, 
            ViewController fromView, ViewController toView,
            Action<Storyboard, TimeSpan> animations,
            bool removeFromView = true)
        {
            if (animations == null)
                throw new ArgumentNullException(nameof(animations));
            var duration   = GetDuration(animation);
            var storyboard = new Storyboard
            {
                Duration = duration
            };
            animations(storyboard, duration);
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
                        else
                            toView?.AddViewAbove(fromView);
                        fromView.RenderTransform = Transform.Identity;
                        fromView.RenderTransformOrigin = new Point(0, 0);
                    }
                    if (toView != null)
                    {
                        toView.RenderTransform       = Transform.Identity;
                        toView.RenderTransformOrigin = new Point(0, 0);
                    }
                    resolve(null, true);
                };
                storyboard.Completed += completed;
                storyboard.Begin();
            });
        }

        protected static Point ConvertToPoint(Position position)
        {
            switch (position)
            {
                case Position.TopLeft:
                    return new Point(0, 0);
                case Position.TopCenter:
                    return new Point(.5, 0);
                case Position.TopRight:
                    return new Point(1, 0);
                case Position.MiddleLeft:
                    return new Point(0, .5);
                case Position.MiddleCenter:
                    return new Point(.5, .5);
                case Position.MiddleRight:
                    return new Point(1, .5);
                case Position.BottomLeft:
                    return new Point(0, 1);
                case Position.BottomCenter:
                    return new Point(.5, 1);
                case Position.BottomRight:
                    return new Point(1, 1);
            }
            throw new InvalidOperationException("Invalid position");
        }
    }
}
