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

        protected static readonly PropertyPath TranslateX =
            new PropertyPath("RenderTransform.X");

        protected static readonly PropertyPath TranslateY =
            new PropertyPath("RenderTransform.Y");

        protected static readonly PropertyPath RotateAngle =
            new PropertyPath("RenderTransform.Angle");

        public abstract Promise Animate(
            ViewController oldView, ViewController newView);

        protected static Promise StartAnimation(Storyboard storyboard, 
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

        protected static TimeSpan GetDuration(IAnimation animation)
        {
            return animation.Duration.GetValueOrDefault(DefaultDuration);
        }

        protected static Promise StartAnimation(AnimationTimeline animation,
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
