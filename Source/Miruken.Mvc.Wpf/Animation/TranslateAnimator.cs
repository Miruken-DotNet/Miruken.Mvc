namespace Miruken.Mvc.Wpf.Animation
{
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Mvc.Animation;

    public class TranslateAnimator : BlendAnimator<Translate>
    {
        public TranslateAnimator(Translate translate)
            : base(translate)
        {
        }

        public override void Animate(
            Storyboard storyboard, ViewController view,
            bool animateOut, bool present)
        {
            int? adjustX  = null;
            int? adjustY  = null;
            var translate = Animation;
            var start = translate.Start ?? Origin.MiddleLeft;

            switch (start)
            {
                case Origin.TopLeft:
                case Origin.TopCenter:
                case Origin.TopRight:
                    adjustY = present ? -1 : 1;
                    break;
                case Origin.BottomLeft:
                case Origin.BottomCenter:
                case Origin.BottomRight:
                    adjustY = present ? 1 : -1;
                    break;
            }

            switch (start)
            {
                case Origin.TopLeft:
                case Origin.MiddleLeft:
                case Origin.BottomLeft:
                case Origin.MiddleCenter:
                    adjustX = present ? -1 : 1;
                    break;
                case Origin.TopRight:
                case Origin.MiddleRight:
                case Origin.BottomRight:
                    adjustX = present ? 1 : -1;
                    break;
            }

            var ease = translate.Behaviors.Find<IEasingFunction>()
                    ?? new CubicEase();

            var property = view.AddTransform(new TranslateTransform());

            if (adjustX.HasValue)
            {
                var translateX = new DoubleAnimation
                {
                    From           = animateOut 
                                   ? 0 
                                   : view.RegionWidth * adjustX,
                    To             = animateOut 
                                   ? view.ActualWidth * adjustX * -1 
                                   : 0,
                    Duration       = storyboard.Duration,
                    EasingFunction = ease
                };
                Configure(translateX, translate, animateOut);
                storyboard.Children.Add(translateX);
                Storyboard.SetTarget(translateX, view);
                Storyboard.SetTargetProperty(translateX,
                    property(TranslateTransform.XProperty));
            }

            if (adjustY.HasValue)
            {
                var translateY = new DoubleAnimation
                {
                    From           = animateOut 
                                   ? 0 
                                   : view.RegionHeight * adjustY,
                    To             = animateOut 
                                   ? view.ActualHeight * adjustY * -1
                                   : 0,
                    Duration       = storyboard.Duration,
                    EasingFunction = ease
                };
                Configure(translateY, translate, animateOut);
                storyboard.Children.Add(translateY);
                Storyboard.SetTarget(translateY, view);
                Storyboard.SetTargetProperty(translateY,
                    property(TranslateTransform.YProperty));
            }
        }
    }
}
