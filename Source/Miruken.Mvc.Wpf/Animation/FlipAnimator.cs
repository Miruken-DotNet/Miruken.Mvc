namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class FlipAnimator : Animator
    {
        public FlipAnimator(Flip flip)
        {
            if (flip == null)
                throw new ArgumentNullException(nameof(flip));
            Flip = flip;
        }

        public Flip Flip { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateFlip(fromView, toView, removeFromView, true);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateFlip(fromView, toView, true, false);
        }

        private Promise AnimateFlip(
             ViewController fromView, ViewController toView,
             bool removeFromView, bool present)
        {
            var promise  = Promise.Empty;
            var flipFrom = fromView != null;
            var flipTo   = toView != null;
            var duration = GetDuration(Flip);
            var middle   = new TimeSpan(duration.Ticks / 2);
            switch (Flip.Mode ?? Mode.InOut)
            {
                case Mode.In:
                    flipFrom = flipFrom && !present;
                    flipTo   = flipTo && present;
                    break;
                case Mode.Out:
                    flipFrom = flipFrom && present;
                    flipTo   = flipTo && !present;
                    break;
            }
            if (flipFrom)
            {
                if (flipTo)
                    toView.HideView();
                else
                    toView?.AddViewBelow(fromView);
                promise = AnimateStory(Flip, fromView, null, hide =>
                {
                    hide.Duration = middle;
                    FadeAnimator.Apply(hide, Flip.Fade, fromView, true);
                    Apply(hide, fromView, true, middle, present);
                }, removeFromView).Then((r, s) =>
                {
                    if (flipTo)
                        toView?.ShowView();
                    else
                        toView?.AddViewAbove(fromView);
                });
            }
            if (flipTo)
            {
                promise = promise.Then((r, s) =>
                    AnimateStory(Flip, null, toView, show =>
                    {
                        if (flipFrom)
                            fromView?.HideView();
                        toView.AddViewAbove(fromView);
                        show.Duration = middle;
                        FadeAnimator.Apply(show, Flip.Fade, toView, false);
                        Apply(show, toView, false, middle, present);
                    }));
                if (flipFrom)
                    promise = promise.Then((r, s) => fromView.ShowView());
            }
            return promise;
        }
        private void Apply(TimelineGroup storyboard,
            ViewController view, bool flipOut, TimeSpan duration,
            bool present)
        {
            var angle = (Flip.Angle ?? 100) * (present ? 1 : -1);
            view.RenderTransformOrigin = new Point(.5, .5);
            var property = view.AddTransform(new SkewTransform());

            var animation = new DoubleAnimation
            {
                Duration  = duration
            };
            Configure(animation, Flip, flipOut);

            if (flipOut)
            {
                animation.To = angle;
            }
            else
            {
                animation.From = -angle;
                animation.To   = 0;
            }

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, 
                property(SkewTransform.AngleXProperty));
            
            var animationY = animation.Clone();
            storyboard.Children.Add(animationY);
            Storyboard.SetTarget(animationY, view);
            Storyboard.SetTargetProperty(animationY,
                property(SkewTransform.AngleYProperty));
        }
    }
}
