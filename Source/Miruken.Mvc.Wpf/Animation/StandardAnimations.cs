namespace Miruken.Mvc.Wpf.Animation
{
    using Callback;
    using Map;
    using Mvc.Animation;

    public class StandardAnimations : Handler
    {
        [Maps]
        public TranslationAnimator Create(Translation translation)
        {
            return new TranslationAnimator(translation);
        }

        [Maps]
        public RollAnimator Create(Roll roll)
        {
            return new RollAnimator(roll);
        }
    }
}
