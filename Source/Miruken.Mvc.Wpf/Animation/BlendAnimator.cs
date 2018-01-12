namespace Miruken.Mvc.Wpf.Animation
{
    using System;
    using System.Windows.Media.Animation;
    using Concurrency;
    using Mvc.Animation;

    public abstract class BlendAnimator<T> : Animator
        where T : IAnimation
    {
        protected BlendAnimator(T animation)
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
            return AnimateStory(Animation, fromView, toView,
                storyboard => 
                    Transition(storyboard,  fromView, toView),
                removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Animation, fromView, toView,
                storyboard => 
                    Transition(storyboard, fromView, toView, false));
        }

        public virtual void Transition(
            Storyboard storyboard,
            ViewController fromView, ViewController toView,
            bool present = true)
        {
            switch (Animation.Mode ?? Mode.InOut)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Animate(storyboard, toView, false, true);
                    }
                    else
                        Animate(storyboard, fromView, true, false);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Animate(storyboard, fromView, true, true);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Animate(storyboard, toView, false, false);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView?.AddViewAbove(fromView);
                    Animate(storyboard, fromView, true, present);
                    Animate(storyboard, toView, false, present);
                    break;
            }
        }

        public abstract void Animate(
            Storyboard storyboard, ViewController view, 
            bool animateOut, bool present);
    }
}
