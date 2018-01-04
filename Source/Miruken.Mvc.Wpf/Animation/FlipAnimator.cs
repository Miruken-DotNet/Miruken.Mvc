namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class FlipAnimator : ChainAnimator<Flip>
    {
        public FlipAnimator(Flip flip) : base(flip)
        {
        }

        protected override void Animate(Storyboard storyboard,
            ViewController view, bool animateOut, TimeSpan duration,
            bool present)
        {
            var angle = (Animation.Angle ?? 100) * (present ? 1 : -1);
            view.RenderTransformOrigin = new Point(.5, .5);
            var property = view.AddTransform(new SkewTransform());

            var animation = new DoubleAnimation
            {
                Duration  = duration
            };
            Configure(animation, Animation, animateOut);

            if (animateOut)
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
