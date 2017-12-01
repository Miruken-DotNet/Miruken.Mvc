namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class FadeAnimator : Animator
    {
        public FadeAnimator(Fade fade)
        {
            if (fade == null)
                throw new ArgumentNullException(nameof(fade));
            Fade = fade;
        }

        public Fade Fade { get; }

        public override Promise Animate(
            ViewController fromView, ViewController toView)
        {
            return Animate(Fade, fromView, toView,
                (storyboard, duration) =>
                    Apply(storyboard, Fade, fromView, toView, duration));
        }

        public static void Apply(TimelineGroup storyboard,
            Fade fade, ViewController fromView, ViewController toView,
            TimeSpan duration, Mode? defaultMmode = null)
        {
            if (fade == null) return;
            switch (fade.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    toView.AddViewAbove(fromView);
                    Apply(storyboard, fade, toView, false, duration);
                    break;
                case Mode.Out:
                    toView?.AddViewBelow(fromView);
                    Apply(storyboard, fade, fromView, true, duration);
                    break;
                case Mode.InOut:
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
