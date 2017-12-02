namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
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

        public override Promise Animate(
            ViewController fromView, ViewController toView)
        {
            return Animate(Translate, fromView, toView,
                (storyboard, duration) =>
                {
                    FadeAnimator.Apply(storyboard, Translate.Fade,
                        fromView, toView, duration, Mode.InOut);
                    Apply(storyboard, Translate, fromView, toView, duration);
                });
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController fromView, 
            ViewController toView, TimeSpan duration)
        {
            if (translate == null) return;
            if (fromView != null && !translate.IsSlide)
                Apply(storyboard, translate, fromView, true, duration);

            if (toView != null)
            {
                Apply(storyboard, translate, toView, false, duration);
                toView.AddViewAbove(fromView);
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController view, bool hide,
            TimeSpan duration)
        {
            double from, to;
            PropertyPath path;

            var property = view.AddTransform(new TranslateTransform());

            switch (translate.Effect)
            {
                case TranslationEffect.PushLeft:
                case TranslationEffect.SlideLeft:
                    from = hide ? 0 : view.RegionWidth;
                    to   = hide ? -view.ActualWidth : 0;
                    path = property(TranslateTransform.XProperty);
                    break;
                case TranslationEffect.PushRight:
                case TranslationEffect.SlideRight:
                    from = hide ? 0 : -view.RegionWidth;
                    to   = hide ? view.ActualWidth : 0;
                    path = property(TranslateTransform.XProperty);
                    break;
                case TranslationEffect.PushDown:
                case TranslationEffect.SlideDown:
                    from = hide ? 0 : -view.RegionHeight;
                    to   = hide ? view.ActualHeight : 0;
                    path = property(TranslateTransform.YProperty);
                    break;
                case TranslationEffect.PushUp:
                case TranslationEffect.SlideUp:
                    from = hide ? 0 : view.RegionHeight;
                    to   = hide ? -view.ActualHeight : 0;
                    path = property(TranslateTransform.YProperty);
                    break;
                default:
                    throw new InvalidOperationException("Invalid translation");
            }

            var animation = new DoubleAnimation
            {
                To             = to,
                From           = from,
                Duration       = duration,
                EasingFunction = translate.Behaviors.Find<IEasingFunction>()
                                 ?? new CubicEase()
            };

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, path);
        }

        private TranslationEffect GetInverseEffect()
        {
            var effect = Translate.Effect;
            switch (effect)
            {
                case TranslationEffect.SlideLeft:
                    return TranslationEffect.SlideRight;
                case TranslationEffect.SlideRight:
                    return TranslationEffect.SlideLeft;
                case TranslationEffect.SlideUp:
                    return TranslationEffect.SlideDown;
                case TranslationEffect.SlideDown:
                    return TranslationEffect.SlideUp;
                case TranslationEffect.PushLeft:
                    return TranslationEffect.PushRight;
                case TranslationEffect.PushRight:
                    return TranslationEffect.PushLeft;
                case TranslationEffect.PushUp:
                    return TranslationEffect.PushDown;
                case TranslationEffect.PushDown:
                    return TranslationEffect.PushUp;
            }
            return effect;
        }
    }
}

