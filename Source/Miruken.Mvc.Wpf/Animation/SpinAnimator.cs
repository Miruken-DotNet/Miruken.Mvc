namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class SpinAnimator : Animator
    {
        public SpinAnimator(Spin spin)
        {
            if (spin == null)
                throw new ArgumentNullException(nameof(spin));
            Spin = spin;
        }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateSpin(fromView, toView, removeFromView, true);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            if (toView.IsChild) toView = null;
            return AnimateSpin(fromView, toView, true, false);
        }

        private Promise AnimateSpin(
            ViewController fromView, ViewController toView,
            bool removeFromView, bool present)
        {
            var promise = fromView != null && removeFromView
                ? AnimateStory(Spin, fromView, null, hide =>
                {
                    var duration  = hide.Duration.TimeSpan;
                    var middle    = new TimeSpan(duration.Ticks / 2);
                    hide.Duration = middle;
                    FadeAnimator.Apply(hide, Spin.Fade, fromView, true);
                    Apply(hide, fromView, true, middle, present);
                })
                : Promise.Empty;
            return toView != null ? promise.Then((r, s) =>
                AnimateStory(Spin, null, toView, show =>
                {
                    toView.AddView();
                    var duration  = show.Duration.TimeSpan;
                    var middle    = new TimeSpan(duration.Ticks / 2);
                    show.Duration = middle;
                    FadeAnimator.Apply(show, Spin.Fade, toView, false);
                    Apply(show, toView, false, middle, present);
                }))
               : promise;
        }

        public Spin Spin { get; }

        private static void Apply(TimelineGroup storyboard,
            ViewController view, bool hide, TimeSpan duration,
            bool present)
        {
            var angle = 360 * (present ? -1 : 1);
            view.RenderTransformOrigin = new Point(.5, .5);
            var property = view.AddTransform(new RotateTransform());
            var rotation = new DoubleAnimation
            {
                From      = hide ? 0 : angle,
                To        = hide ? -angle : 0,
                Duration  = duration
            };
            storyboard.Children.Add(rotation);
            Storyboard.SetTarget(rotation, view);
            Storyboard.SetTargetProperty(rotation,
                property(RotateTransform.AngleProperty));

            property = view.AddTransform(new ScaleTransform());
            var scaleX = new DoubleAnimation
            {
                From      = hide ? 1 : 0,
                To        = hide ? 0 : 1,
                Duration  = duration
            };
            storyboard.Children.Add(scaleX);
            Storyboard.SetTarget(scaleX, view);
            Storyboard.SetTargetProperty(scaleX,
                property(ScaleTransform.ScaleXProperty));

            var scaleY = scaleX.Clone();
            storyboard.Children.Add(scaleY);
            Storyboard.SetTarget(scaleY, view);
            Storyboard.SetTargetProperty(scaleY,
                property(ScaleTransform.ScaleYProperty));
        }
    }
}
