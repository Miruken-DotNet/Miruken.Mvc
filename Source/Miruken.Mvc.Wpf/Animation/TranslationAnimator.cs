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
        public TranslationAnimator(Translation translation)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));
            Translation = translation;
        }

        public Translation Translation { get; }

        public override Promise Animate(
            ViewController oldView, ViewController newView)
        {
            var storyboard = new Storyboard();
            var duration   = Translation.Duration.GetValueOrDefault(DefaultDuration);

            if (oldView != null && !Translation.IsSlide)
                CreateAnimation(storyboard, oldView, true, duration);

            if (newView != null)
            {
                CreateAnimation(storyboard, newView, false, duration);
                newView.AddViewAfter(oldView);
            }

            return StartAnimation(storyboard, oldView, newView);
        }

        private void CreateAnimation(TimelineGroup storyboard,
            ViewController view, bool old, TimeSpan duration)
        {
            double from , to;
            PropertyPath path;

            view.RenderTransform = new TranslateTransform();

            switch (Translation.Effect)
            {
                case TranslationEffect.PushLeft:
                case TranslationEffect.SlideLeft:
                    from = old ? 0 : view.RegionWidth;
                    to   = old ? -view.ActualWidth : 0;
                    path = TranslateX;
                    break;
                case TranslationEffect.PushRight:
                case TranslationEffect.SlideRight:
                    from = old ? 0 : -view.RegionWidth;
                    to   = old ? view.ActualWidth : 0;
                    path = TranslateX;
                    break;
                case TranslationEffect.PushDown:
                case TranslationEffect.SlideDown:
                    from = old ? 0 : -view.RegionHeight;
                    to   = old ? view.ActualHeight : 0;
                    path = TranslateY;
                    break;
                case TranslationEffect.PushUp:
                case TranslationEffect.SlideUp:
                    from = old ? 0 : view.RegionHeight;
                    to   = old ? -view.ActualHeight : 0;
                    path = TranslateY;
                    break;
                default:
                    throw new InvalidOperationException("Invalid translation");
            }

            var animation = new DoubleAnimation
            {
                To             = to,
                From           = from,
                Duration       = duration,
                EasingFunction = Translation.Behaviors.Find<IEasingFunction>()
                               ?? new CubicEase()
            };

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, path);
        }
    }
}
