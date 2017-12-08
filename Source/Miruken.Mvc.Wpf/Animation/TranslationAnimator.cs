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
            return null;
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController fromView, 
            ViewController toView, TimeSpan duration,
            Mode? defaultMode = null)
        {
            if (translate == null) return;
            switch (translate.Mode ?? defaultMode ?? Mode.InOut)
            {
                case Mode.In:
                    toView.AddViewAbove(fromView);
                    Apply(storyboard, translate, toView, false, duration);
                    break;
                case Mode.Out:
                    toView?.AddViewBelow(fromView);
                    Apply(storyboard, translate, fromView, true, duration);
                    break;
                case Mode.InOut:
                    toView.AddViewAbove(fromView);
                    Apply(storyboard, translate, toView, false, duration);
                    Apply(storyboard, translate, fromView, true, duration);
                    break;
            }
        }

        public static void Apply(TimelineGroup storyboard,
            Translate translate, ViewController view, bool hide,
            TimeSpan duration)
        {
            DoubleAnimation translateX = null;
            DoubleAnimation translateY = null;

            var start = translate.Start ?? Origin.MiddleLeft;

            switch (start)
            {
                case Origin.TopLeft:
                case Origin.TopCenter:
                case Origin.TopRight:
                    translateY = new DoubleAnimation
                    {
                        From = hide ? 0 : -view.RegionHeight,
                        To   = hide ? view.ActualHeight : 0
                    };
                    break;
                case Origin.BottomLeft:
                case Origin.BottomCenter:
                case Origin.BottomRight:
                    translateY = new DoubleAnimation
                    {
                        From = hide ? 0 : view.RegionHeight,
                        To   = hide ? -view.ActualHeight : 0
                    };
                    break;
            }

            switch (start)
            {
                case Origin.TopLeft:
                case Origin.MiddleLeft:
                case Origin.BottomLeft:
                    translateX = new DoubleAnimation
                    {
                        From = hide ? 0 : -view.RegionWidth,
                        To   = hide ? view.ActualWidth : 0
                    };
                    break;
                case Origin.TopRight:
                case Origin.MiddleRight:
                case Origin.BottomRight:
                    translateX = new DoubleAnimation
                    {
                        From = hide ? 0 : view.RegionWidth,
                        To   = hide ? -view.ActualWidth : 0
                    };
                    break;
            }

            if (translateX == null && translateY == null)
                return;

            var ease = translate.Behaviors.Find<IEasingFunction>()
                    ?? new CubicEase();

            var property = view.AddTransform(new TranslateTransform());

            if (translateX != null)
            {
                translateX.Duration = duration;
                translateX.EasingFunction = ease;
                storyboard.Children.Add(translateX);
                Storyboard.SetTarget(translateX, view);
                Storyboard.SetTargetProperty(translateX,
                    property(TranslateTransform.XProperty));
            }

            if (translateY != null)
            {
                translateY.Duration = duration;
                translateY.EasingFunction = ease;
                storyboard.Children.Add(translateY);
                Storyboard.SetTarget(translateY, view);
                Storyboard.SetTargetProperty(translateY,
                    property(TranslateTransform.YProperty));
            }
        }
    }
}
