namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media.Animation;
    using Concurrency;

    public abstract class Animator : IAnimator
    {
        protected readonly TimeSpan DefaultDuration =
            TimeSpan.FromMilliseconds(400);

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
    }
}
