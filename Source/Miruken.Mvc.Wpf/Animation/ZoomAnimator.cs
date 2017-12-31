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
            return AnimateStory(Zoom, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Zoom.Fade,
                    fromView, toView, true, Zoom.Mode);
                Apply(storyboard, Zoom, fromView, toView);
            }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Zoom, fromView, toView, storyboard =>
            {
                FadeAnimator.Apply(storyboard, Zoom.Fade,
                    fromView, toView, false, Zoom.Mode);
                Apply(storyboard, Zoom, fromView, toView, false);
            });
        }

        public static void Apply(TimelineGroup storyboard,
            Zoom zoom, ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMmode = null)
        {
            if (zoom == null) return;
            switch (zoom.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, zoom, toView, false);
                    }
                    else
                        Apply(storyboard, zoom, fromView, true);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, zoom, fromView, true);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, zoom, toView, false);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView.AddViewAbove(fromView);
                    Apply(storyboard, zoom, toView, false);
                    Apply(storyboard, zoom, fromView, true);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Zoom zoom, ViewController view, bool zoomOut)
        {
            if (zoom == null || view == null) return;

            var origin = zoom.Origin ?? Origin.MiddleCenter;
            view.RenderTransformOrigin = ConvertToPoint(origin);
            var property = view.AddTransform(new ScaleTransform());

            var animation = new DoubleAnimation
            {
                Duration = storyboard.Duration
            };
            Configure(animation, zoom);

            if (zoomOut)
            {
                animation.To = zoom.Scale ?? 0;
            }
            else
            {
                animation.From = zoom.Scale ?? 0;
                animation.To   = 1;
            }

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
