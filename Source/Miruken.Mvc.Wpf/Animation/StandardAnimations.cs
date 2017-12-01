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
    }
}
