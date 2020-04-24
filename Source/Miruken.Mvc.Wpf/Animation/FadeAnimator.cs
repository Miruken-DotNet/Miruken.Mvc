namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows;
    using System.Windows.Media.Animation;
    using Mvc.Animation;
    using IAnimation = Mvc.Animation.IAnimation;

    public class FadeAnimator : BlendAnimator<Fade>
    {
        private static readonly PropertyPath Opacity =
            new PropertyPath(UIElement.OpacityProperty);

        public FadeAnimator(Fade fade) : base(fade)
        {
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
            if (view == null) return;
            var animation = new DoubleAnimation
            {
                To       = animateOut ? 0 : 1,
                Duration = storyboard.Duration
            };
            if (!animateOut) animation.From = 0;
            Configure(animation, Animation, animateOut);
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, Opacity);
        }

        public static FadeAnimator For(IAnimation animation)
        {
            return For(animation?.Fade);
        }

        public static FadeAnimator For(Fade fade)
        {
            return fade != null ? new FadeAnimator(fade) : null;
        }
    }
}
