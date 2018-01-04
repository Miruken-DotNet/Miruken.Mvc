namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class SpinAnimator : DualAnimator<Spin>
    {
        public SpinAnimator(Spin spin) : base(spin)
        {          
        }

        protected override void Apply(Storyboard storyboard,
            ViewController view, bool animateOut,
            TimeSpan duration, bool present)
        {
            var angle = 360 * (present ? -1 : 1);
            view.RenderTransformOrigin = new Point(.5, .5);
            var property = view.AddTransform(new RotateTransform());
            var rotation = new DoubleAnimation
            {
                From      = animateOut ? 0 : angle,
                To        = animateOut ? -angle : 0,
                Duration  = duration
            };
            Configure(rotation, Animation, animateOut);
            storyboard.Children.Add(rotation);
            Storyboard.SetTarget(rotation, view);
            Storyboard.SetTargetProperty(rotation,
                property(RotateTransform.AngleProperty));

            property = view.AddTransform(new ScaleTransform());
            var scaleX = new DoubleAnimation
            {
                From      = animateOut ? 1 : 0,
                To        = animateOut ? 0 : 1,
                Duration  = duration
            };
            Configure(scaleX, Animation, animateOut);
            storyboard.Children.Add(scaleX);
            Storyboard.SetTarget(scaleX, view);
            Storyboard.SetTargetProperty(scaleX,
                property(ScaleTransform.ScaleXProperty));

            var scaleY = scaleX.Clone();
            Configure(scaleY, Animation, animateOut);
            storyboard.Children.Add(scaleY);
            Storyboard.SetTarget(scaleY, view);
            Storyboard.SetTargetProperty(scaleY,
                property(ScaleTransform.ScaleYProperty));
        }
    }
}
