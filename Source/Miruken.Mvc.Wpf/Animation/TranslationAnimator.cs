namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class TranslationAnimator : Animator
    {
        public TranslationAnimator(Translate translate)
        {
            if (translate == null)
                throw new ArgumentNullException(nameof(translate));
            Translate = translate;
        }

        public Translate Translate { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateStory(Translate, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Translate.Fade,
                        fromView, toView, duration, true, Translate.Mode);
                    Apply(storyboard, Translate, fromView, toView, duration);
                }, removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Translate, fromView, toView,      
            (storyboard, duration) =>
            {
                FadeAnimator.Apply(storyboard, Translate.Fade,
                    fromView, toView, duration, true, Translate.Mode);
                Apply(storyboard, Translate, fromView, toView, duration, false);
            });
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController fromView, 
            ViewController toView, TimeSpan duration,
            bool present = true, Mode? defaultMode = null)
        {
            if (translate == null) return;
            switch (translate.Mode ?? defaultMode ?? Mode.InOut)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Apply(storyboard, translate, toView, true, false, duration);
                    }
                    else
                        Apply(storyboard, translate, fromView, false, true, duration);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Apply(storyboard, translate, fromView, true, true, duration);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Apply(storyboard, translate, toView, false, false, duration);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView.AddViewAbove(fromView);
                    Apply(storyboard, translate, toView, present, false, duration);
                    Apply(storyboard, translate, fromView, present, true, duration);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController view,
            bool present, bool hide, TimeSpan duration)
        {
            int? adjustX = null;
            int? adjustY = null;
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
                    From           = hide ? 0 : view.RegionWidth * adjustX,
                    To             = hide ? view.ActualWidth * adjustX * -1 : 0,
                    Duration       = duration,
                    EasingFunction = ease
                };
                storyboard.Children.Add(translateX);
                Storyboard.SetTarget(translateX, view);
                Storyboard.SetTargetProperty(translateX,
                    property(TranslateTransform.XProperty));
            }

            if (adjustY.HasValue)
            {
                var translateY = new DoubleAnimation
                {
                    From           = hide ? 0 : view.RegionHeight * adjustY,
                    To             = hide ? view.ActualHeight * adjustY * -1 : 0,
                    Duration       = duration,
                    EasingFunction = ease
                };
                storyboard.Children.Add(translateY);
                Storyboard.SetTarget(translateY, view);
                Storyboard.SetTargetProperty(translateY,
                    property(TranslateTransform.YProperty));
            }
        }
    }
}
