namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class ZoomAnimator : Animator
    {
        public ZoomAnimator(Zoom zoom)
        {
            if (zoom == null)
                throw new ArgumentNullException(nameof(zoom));
            Zoom = zoom;
        }

        public Zoom Zoom { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Zoom, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Zoom.Fade,
                        fromView, toView, duration, true, Zoom.Mode);
                    Apply(storyboard, Zoom, fromView, toView, duration);
                }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Zoom, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Zoom.Fade,
                        fromView, toView, duration, false, Zoom.Mode);
                    Apply(storyboard, Zoom, fromView, toView, duration, false);
                });
        }

        public static void Apply(TimelineGroup storyboard,
            Zoom zoom, ViewController fromView, ViewController toView,
            TimeSpan duration, bool present = true,
            Mode? defaultMmode = null)
        {
            if (zoom == null) return;
            switch (zoom.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, zoom, toView, false, duration);
                    }
                    else
                        Apply(storyboard, zoom, fromView, true, duration);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, zoom, fromView, true, duration);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, zoom, toView, false, duration);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView.AddViewAbove(fromView);
                    Apply(storyboard, zoom, toView, false, duration);
                    Apply(storyboard, zoom, fromView, true, duration);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Zoom zoom, ViewController view, bool zoomOut,
            TimeSpan duration)
        {
            if (zoom == null || view == null) return;

            var origin = zoom.Origin ?? Origin.MiddleCenter;
            view.RenderTransformOrigin = ConvertToPoint(origin);
            var property = view.AddTransform(new ScaleTransform());

            var animation = new DoubleAnimation
            {
                To             = zoomOut ? 0 : 1,
                Duration       = duration,
                EasingFunction = zoom.Behaviors.Find<IEasingFunction>()
            };
            if (!zoomOut) animation.From = 0;

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, 
                property(ScaleTransform.ScaleXProperty));

            var animationY = animation.Clone();
            storyboard.Children.Add(animationY);
            Storyboard.SetTarget(animationY, view);
            Storyboard.SetTargetProperty(animation,
                property(ScaleTransform.ScaleYProperty));
        }
    }
}
