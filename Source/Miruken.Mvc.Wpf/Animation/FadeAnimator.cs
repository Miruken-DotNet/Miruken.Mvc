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
                storyboard => Apply(storyboard, Fade, fromView, toView),
                removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Fade, fromView, toView,
                storyboard => Apply(storyboard, Fade, fromView, toView, false));
        }

        public static void Apply(TimelineGroup storyboard,
            Fade fade, ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMmode = null)
        {
            if (fade == null) return;
            switch (fade.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, fade, toView, false);
                    }
                    else
                        Apply(storyboard, fade, fromView, true);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, fade, fromView, true);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, fade, toView, false);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView?.AddViewAbove(fromView);
                    Apply(storyboard, fade, toView, false);
                    Apply(storyboard, fade, fromView, true);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Fade fade, ViewController view, bool fadeOut)
        {
            if (fade == null || view == null) return;
            var animation = new DoubleAnimation
            {
                To       = fadeOut ? 0 : 1,
                Duration = storyboard.Duration
            };
            if (!fadeOut) animation.From = 0;
            Configure(animation, fade, fadeOut);
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, Opacity);
        }
    }
}
