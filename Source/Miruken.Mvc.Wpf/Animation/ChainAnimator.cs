namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public abstract class ChainAnimator<T> : Animator
        where T : IAnimation
    {
        protected ChainAnimator(T animation)
        {
            if (animation == null)
                throw new ArgumentNullException(nameof(animation));
            Animation = animation;
        }

        public T Animation { get; }

        public override Promise Present(
            ViewController fromView, ViewController toView,
            bool removeFromView)
        {
            return AnimateDual(fromView, toView, removeFromView, true);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateDual(fromView, toView, true, false);
        }

        private Promise AnimateDual(
            ViewController fromView, ViewController toView,
            bool removeFromView, bool present)
        {
            var promise     = Promise.Empty;
            var animateFrom = fromView != null;
            var animationTo = toView != null;
            var duration    = GetDuration(Animation);
            var middle      = new TimeSpan(duration.Ticks / 2);
            switch (Animation.Mode ?? Mode.InOut)
            {
                case Mode.In:
                    animateFrom = animateFrom && !present;
                    animationTo = animationTo && present;
                    break;
                case Mode.Out:
                    animateFrom = animateFrom && present;
                    animationTo = animationTo && !present;
                    break;
            }
            if (animateFrom)
            {
                if (animationTo)
                    toView.HideView();
                else
                    toView?.AddViewBelow(fromView);
                promise = AnimateStory(Animation, fromView, null, hide =>
                {
                    hide.Duration = middle;
                    FadeAnimator.Apply(hide, Animation.Fade, fromView, true);
                    Animate(hide, fromView, true, middle, present);
                }, removeFromView).Then((r, s) =>
                {
                    if (animationTo)
                        toView?.ShowView();
                    else
                        toView?.AddViewAbove(fromView);
                });
            }
            if (animationTo)
            {
                promise = promise.Then((r, s) =>
                    AnimateStory(Animation, null, toView, show =>
                    {
                        if (animateFrom)
                            fromView?.HideView();
                        toView.AddViewAbove(fromView);
                        show.Duration = middle;
                        FadeAnimator.Apply(show, Animation.Fade, toView, false);
                        Animate(show, toView, false, middle, present);
                    }));
                if (animateFrom)
                    promise = promise.Then((r, s) => fromView.ShowView());
            }
            return promise;
        }

        protected abstract void Animate(Storyboard storyboard,
            ViewController view, bool animateOut, TimeSpan duration,
            bool present);
    }
}
