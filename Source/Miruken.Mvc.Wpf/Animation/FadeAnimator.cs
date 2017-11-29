namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
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
            ViewController oldView, ViewController newView)
        {
            if (oldView != null)
            {
                newView.AddViewBefore(oldView);
                return CreateAnimation(oldView, null, 0);
            }
            newView.AddView();
            return CreateAnimation(newView, 0, 1, false);
        }

        private Promise CreateAnimation(
            ViewController view, double? from, double? to,
            bool remove = true)
        {
            var animation = new DoubleAnimation
            {
                Duration       = GetDuration(Fade),
                EasingFunction = Fade.Behaviors.Find<IEasingFunction>()
            };
            if (from.HasValue) animation.From = from.Value;
            if (to.HasValue) animation.To = to.Value;
            return StartAnimation(animation, view, UIElement.OpacityProperty, remove);
        }
    }
}
