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

        public Spin Spin { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateSpin(fromView, toView, removeFromView, true);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateSpin(fromView, toView, true, false);
        }

        private Promise AnimateSpin(
            ViewController fromView, ViewController toView,
            bool removeFromView, bool present)
        {
            var promise  = Promise.Empty;
            var spinFrom = fromView != null;
            var spinTo   = toView != null;
            var duration = GetDuration(Spin);
            var middle   = new TimeSpan(duration.Ticks / 2);
            switch (Spin.Mode ?? Mode.InOut)
            {
                case Mode.In:
                    spinFrom = spinFrom && !present;
                    spinTo   = spinTo && present;
                    break;
                case Mode.Out:
                    spinFrom = spinFrom && present;
                    spinTo   = spinTo && !present;
                    break;
            }
            if (spinFrom)
            {
                if (spinTo)
                    toView.HideView();
                else
                    toView?.AddViewBelow(fromView);
                promise = AnimateStory(Spin, fromView, null, hide =>
                {
                    hide.Duration = middle;
                    FadeAnimator.Apply(hide, Spin.Fade, fromView, true);
                    Apply(hide, fromView, true, middle, present);
                }, removeFromView).Then((r, s) =>
                {
                    if (spinTo)
                        toView?.ShowView();
                    else
                        toView?.AddViewAbove(fromView);
                });
            }
            if (spinTo)
            {
                promise = promise.Then((r, s) =>
                    AnimateStory(Spin, null, toView, show =>
                    {
                        if (spinFrom)
                            fromView?.HideView();
                        toView.AddViewAbove(fromView);
                        show.Duration = middle;
                        FadeAnimator.Apply(show, Spin.Fade, toView, false);
                        Apply(show, toView, false, middle, present);
                    }));
                if (spinFrom)
                    promise = promise.Then((r, s) => fromView.ShowView());
            }
            return promise;
        }

        private void Apply(TimelineGroup storyboard,
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
            Configure(rotation, Spin, hide);
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
            Configure(scaleX, Spin, hide);
            storyboard.Children.Add(scaleX);
            Storyboard.SetTarget(scaleX, view);
            Storyboard.SetTargetProperty(scaleX,
                property(ScaleTransform.ScaleXProperty));

            var scaleY = scaleX.Clone();
            Configure(scaleY, Spin, hide);
            storyboard.Children.Add(scaleY);
            Storyboard.SetTarget(scaleY, view);
            Storyboard.SetTargetProperty(scaleY,
                property(ScaleTransform.ScaleYProperty));
        }
    }
}
