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
        private const double DefaultDuration = 600;
        private static readonly PropertyPath TransformX =
            new PropertyPath("(RenderTransform).(TranslateTransform.X)");
        
        public TranslationAnimator(Translation translation)
        {
            Translation = translation;
        }

        public Translation Translation { get; }

        public Promise Animate(ViewController oldView, ViewController newView)
        {
            var storyboard = new Storyboard();
            var duration = Translation.Duration.GetValueOrDefault(DefaultDuration);
            if (oldView != null)
            {
                var doubleAnimation = new DoubleAnimation
                {
                    To             = oldView.ActualWidth,
                    Duration       = TimeSpan.FromMilliseconds(duration),
                    EasingFunction = new CubicEase()
                };
                storyboard.Children.Add(doubleAnimation);
                Storyboard.SetTarget(doubleAnimation, oldView);
                Storyboard.SetTargetProperty(doubleAnimation, TransformX);
                oldView.RenderTransform = new TranslateTransform();
            }
            if (newView != null)
            {
                var doubleAnimation = new DoubleAnimation
                {
                    To             = 0,
                    From           = -newView.ExpectedWidth,
                    Duration       = TimeSpan.FromMilliseconds(duration),
                    EasingFunction = new CubicEase()

                };
                storyboard.Children.Add(doubleAnimation);
                Storyboard.SetTarget(doubleAnimation, newView);
                Storyboard.SetTargetProperty(doubleAnimation, TransformX);
                newView.RenderTransform = new TranslateTransform();
                newView.AddToParentAfter(oldView);
            }
            return new Promise<object>((resolve, reject) =>
            {
                storyboard.Completed += (s, e) =>
                {
                    oldView?.RemoveFromParent();
                    if (newView != null)
                        newView.RenderTransform = null;
                    storyboard.Remove();
                    resolve(null, true);
                };
                storyboard.Begin();
            });
        }
    }
}
