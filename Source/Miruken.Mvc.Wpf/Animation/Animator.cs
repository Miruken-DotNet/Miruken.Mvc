namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public abstract class Animator : IAnimator
    {
        protected static readonly TimeSpan DefaultDuration =
            TimeSpan.FromMilliseconds(400);

        protected static readonly PropertyPath Opacity =
            new PropertyPath(UIElement.OpacityProperty);

        protected static readonly PropertyPath TranslateX =
            new PropertyPath("RenderTransform.X");

        protected static readonly PropertyPath TranslateY =
            new PropertyPath("RenderTransform.Y");

        protected static readonly PropertyPath RotateAngle =
            new PropertyPath("RenderTransform.Angle");

        public abstract Promise Animate(
            ViewController oldView, ViewController newView);

        protected static TimeSpan GetDuration(IAnimation animation)
        {
            return animation.Duration.GetValueOrDefault(DefaultDuration);
        }

        protected static void ApplyFade(TimelineGroup storyboard,
            IAnimation transition, DependencyObject oldView,
            DependencyObject newView, TimeSpan duration)
        {
            var fade = transition.Fade;
            if (fade == null) return;
            var ease = fade.Behaviors.Find<IEasingFunction>();
            if (oldView != null)
            {
                var animation = new DoubleAnimation
                {
                    To             = 0,
                    Duration       = duration,
                    EasingFunction = ease
                };
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, oldView);
                Storyboard.SetTargetProperty(animation, Opacity);
            }
            if (newView != null)
            {
                var animation = new DoubleAnimation
                {
                    From           = 0,
                    To             = 1,
                    Duration       = duration,
                    EasingFunction = ease
                };
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, newView);
                Storyboard.SetTargetProperty(animation, Opacity);
            }
        }

        protected static Promise Animate(Storyboard storyboard, 
            ViewController oldView, ViewController newView,
            Action cleanup = null)
        {
            return new Promise<object>((resolve, reject) =>
            {
                EventHandler completed = null;
                completed = (s, e) =>
                {
                    storyboard.Completed -= completed;
                    storyboard.Remove();
                    oldView?.RemoveView();
                    if (newView != null)
                        newView.RenderTransform = null;
                    cleanup?.Invoke();
                    resolve(null, true);
                };
                storyboard.Completed += completed;
                storyboard.Begin();
            });
        }

        protected static Promise Animate(AnimationTimeline animation,
            ViewController view, DependencyProperty property, bool remove = true,
            Action cleanup = null)
        {
            return new Promise<object>((resolve, reject) =>
            {
                EventHandler completed = null;
                completed = (s, e) =>
                {
                    animation.Completed -= completed;
                    view.BeginAnimation(property, null);
                    if (remove) view.RemoveView();
                    cleanup?.Invoke();
                    resolve(null, true);
                };
                animation.Completed += completed;
                view.BeginAnimation(property, animation);
            });
        }
    }
}
