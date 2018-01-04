namespace Miruken.Mvc.Wpf.Animation
{
    using Callback;
    using Map;
    using Mvc.Animation;

    public class StandardAnimations : Handler
    {
        [Maps]
        public FadeAnimator Create(Fade fade)
        {
            return new FadeAnimator(fade);
        }

        [Maps]
        public TranslationAnimator Create(Translate translate)
        {
            return new TranslationAnimator(translate);
        }

        [Maps]
        public RollAnimator Create(Roll roll)
        {
            return new RollAnimator(roll);
        }

        [Maps]
        public ZoomAnimator Create(Zoom zoom)
        {
            return new ZoomAnimator(zoom);
        }

        [Maps]
        public SplitAnimator Create(Split split)
        {
            return new SplitAnimator(split);
        }

        [Maps]
        public SpinAnimator Create(Spin spin)
        {
            return new SpinAnimator(spin);
        }

        [Maps]
        public FlipAnimator Create(Flip flip)
        {
            return new FlipAnimator(flip);
        }

        [Maps]
        public WipeAnimator Create(Wipe wipe)
        {
            return new WipeAnimator(wipe);
        }

        [Maps]
        public WipeRotateAnimator Create(WipeRotate wipe)
        {
            return new WipeRotateAnimator(wipe);
        }
    }
}
