namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public class TranslationAnimator : IAnimator
    {
        private readonly TimeSpan DefaultDuration =
            TimeSpan.FromMilliseconds(400);

        private static readonly PropertyPath TransformX =
            new PropertyPath("RenderTransform.X");

        private static readonly PropertyPath TransformY =
            new PropertyPath("RenderTransform.Y");

        public TranslationAnimator(Translation translation)
        {
            Translation = translation;
        }

        public Translation Translation { get; }

        public Promise Animate(ViewController oldView, ViewController newView)
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

            return new Promise<object>((resolve, reject) =>
            {
                EventHandler completed = null;
                completed = (s, e) =>
                {
                    storyboard.Completed -= completed;
                    oldView?.RemoveView();
                    if (newView != null)
                        newView.RenderTransform = null;
                    storyboard.Remove();
                    resolve(null, true);
                };
                storyboard.Completed += completed;
                storyboard.Begin();
            });
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
                    path = TransformX;
                    break;
                case TranslationEffect.PushRight:
                case TranslationEffect.SlideRight:
                    from = old ? 0 : -view.RegionWidth;
                    to   = old ? view.ActualWidth : 0;
                    path = TransformX;
                    break;
                case TranslationEffect.PushDown:
                case TranslationEffect.SlideDown:
                    from = old ? 0 : -view.RegionHeight;
                    to   = old ? view.ActualHeight : 0;
                    path = TransformY;
                    break;
                case TranslationEffect.PushUp:
                case TranslationEffect.SlideUp:
                    from = old ? 0 : view.RegionHeight;
                    to   = old ? -view.ActualHeight : 0;
                    path = TransformY;
                    break;
                default:
                    throw new InvalidOperationException("Invalid translation");
            }

            var animation = new DoubleAnimation
            {
                To             = to,
                From           = from,
                Duration       = duration,
                EasingFunction = new CubicEase()
            };

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, view);
            Storyboard.SetTargetProperty(animation, path);
        }
    }
}
