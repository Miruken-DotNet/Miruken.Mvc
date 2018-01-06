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
                storyboard => Animate(storyboard, Animation, 
                fromView, toView), removeFromView);
        }

        public override Promise Dismiss(
            ViewController fromView, ViewController toView)
        {
            return AnimateStory(Animation, fromView, toView,
                storyboard => Animate(storyboard, Animation,
                fromView, toView, false));
        }

        public void Animate(Storyboard storyboard,
            T animation, ViewController fromView, ViewController toView,
            bool present = true, Mode? defaultMmode = null)
        {
            if (animation == null) return;
            switch (animation.Mode ?? defaultMmode ?? Mode.In)
            {
                case Mode.In:
                    if (present)
                    {
                        toView.AddViewAbove(fromView);
                        Animate(storyboard, animation, toView, false);
                    }
                    else
                        Animate(storyboard, animation, fromView, true);
                    break;
                case Mode.Out:
                    if (present)
                    {
                        toView?.AddViewBelow(fromView);
                        Animate(storyboard, animation, fromView, true);
                    }
                    else
                    {
                        toView?.AddViewAbove(fromView);
                        Animate(storyboard, animation, toView, false);
                    }
                    break;
                case Mode.InOut:
                    if (present)
                        toView?.AddViewAbove(fromView);
                    Animate(storyboard, animation, toView, false);
                    Animate(storyboard, animation, fromView, true);
                    break;
            }
        }

        protected abstract void Animate(Storyboard storyboard,
            T animation, ViewController view, bool animateOut);
    }
}
