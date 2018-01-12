namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class ZoomAnimator : BlendAnimator<Zoom>
    {
        public ZoomAnimator(Zoom zoom) : base(zoom)
        {
        }

        public override void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            Fade(storyboard, Animation.Fade, fromView, toView,
                present, Animation.Mode ?? Mode.InOut);
            base.Transition(storyboard, fromView, toView, present);
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
            if (view == null) return;

            var zoom   = Animation;
            var origin = zoom.Origin ?? Origin.MiddleCenter;
            view.RenderTransformOrigin = ConvertToPoint(origin);
            var property = view.AddTransform(new ScaleTransform());

            var animation = new DoubleAnimation
            {
                Duration = storyboard.Duration
            };
            Configure(animation, zoom, animateOut);

            if (animateOut)
            {
                animation.To = zoom.Scale ?? 0;
            }
            else
            {
                animation.From = zoom.Scale ?? 0;
                animation.To   = 1;
            }

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, 
                property(ScaleTransform.ScaleXProperty));

            var animationY = animation.Clone();
            storyboard.Children.Add(animationY);
            Storyboard.SetTarget(animationY, view);
            Storyboard.SetTargetProperty(animationY,
                property(ScaleTransform.ScaleYProperty));
        }
    }
}
