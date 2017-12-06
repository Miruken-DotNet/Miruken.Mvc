namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class FadeAnimator : Animator
    {
        private static readonly PropertyPath Opacity =
            new PropertyPath(UIElement.OpacityProperty);

        public FadeAnimator(Fade fade)
        {
            if (fade == null)
                throw new ArgumentNullException(nameof(fade));
            Fade = fade;
        }

        public Fade Fade { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Fade, fromView, toView,
                (storyboard, duration) =>
                    Apply(storyboard, Fade, fromView, toView, duration),
                removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Fade, fromView, toView,
                (storyboard, duration) =>
                    Apply(storyboard, Fade, fromView, toView, duration, false));
        }

        public static void Apply(TimelineGroup storyboard,
            Fade fade, ViewController fromView, ViewController toView,
            TimeSpan duration, bool present = true,
            Mode? defaultMmode = null)
        {
            if (fade == null) return;
            switch (fade.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, fade, toView, false, duration);
                    }
                    else
                        Apply(storyboard, fade, fromView, true, duration);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, fade, fromView, true, duration);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, fade, toView, false, duration);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView.AddViewAbove(fromView);
                    Apply(storyboard, fade, toView, false, duration);
                    Apply(storyboard, fade, fromView, true, duration);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Fade fade, ViewController view, bool fadeOut,
            TimeSpan duration)
        {
            if (fade == null || view == null) return;
            var animation = new DoubleAnimation
            {
                To             = fadeOut ? 0 : 1,
                Duration       = duration,
                EasingFunction = fade.Behaviors.Find<IEasingFunction>()
            };
            if (!fadeOut) animation.From = 0;
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, Opacity);
        }
    }
}
