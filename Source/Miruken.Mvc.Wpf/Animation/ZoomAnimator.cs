namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
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

        public override Promise Animate(
            ViewController oldView, ViewController newView)
        {
            var storyboard = new Storyboard();
            var duration   = GetDuration(Zoom);
            var newOrigin  = newView?.RenderTransformOrigin;

            ApplyFade(storyboard, Zoom, oldView, newView, duration);

            if (oldView != null)
            {
                newView?.AddViewBefore(oldView);
                AddAnimation(storyboard, oldView, null, 0, duration);
            }
            else if (newView != null)
            {
                newView.AddView();
                AddAnimation(storyboard, newView, 0, 1, duration);
            }

            return Animate(storyboard, oldView, newView, () =>
            {
                if (newView != null && newOrigin.HasValue)
                    newView.RenderTransformOrigin = newOrigin.Value;
            });
        }

        private void AddAnimation(TimelineGroup storyboard,
            UIElement view, double? from, double? to, TimeSpan duration)
        {
            view.RenderTransform       = new ScaleTransform();
            view.RenderTransformOrigin = new Point(.5, .5);

            var animation = new DoubleAnimation
            {
                Duration       = duration,
                EasingFunction = Zoom.Behaviors.Find<IEasingFunction>()
            };
            if (from.HasValue) animation.From = from.Value;
            if (to.HasValue) animation.To = to.Value;

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, ScaleX);

            var animationY = animation.Clone();
            storyboard.Children.Add(animationY);
            Storyboard.SetTarget(animationY, view);
            Storyboard.SetTargetProperty(animation, ScaleY);
        }
    }
}
