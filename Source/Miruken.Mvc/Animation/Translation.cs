namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public enum TranslationEffect
    {
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        PushLeft,
        PushRight,
        PushUp,
        PushDown
    }

    public class Translation : Animation
    {
        public Translation(TranslationEffect effect)
        {
            Effect = effect;
        }

        public TranslationEffect Effect { get; }

        public bool IsSlide
        {
            get
            {
                switch (Effect)
                {
                    case TranslationEffect.SlideLeft:
                    case TranslationEffect.SlideRight:
                    case TranslationEffect.SlideDown:
                    case TranslationEffect.SlideUp:
                        return true;
                    default:
                        return false;
                }
            }    
        }

        public override IAnimation CreateInverse()
        {
            return new Translation(GetInverseEffect())
            {
                Duration = Duration
            };
        }

        private TranslationEffect GetInverseEffect()
        {
            switch (Effect)
            {
                case TranslationEffect.SlideLeft:
                    return TranslationEffect.SlideRight;
                case TranslationEffect.SlideRight:
                    return TranslationEffect.SlideLeft;
                case TranslationEffect.SlideUp:
                    return TranslationEffect.SlideDown;
                case TranslationEffect.SlideDown:
                    return TranslationEffect.SlideUp;
                case TranslationEffect.PushLeft:
                    return TranslationEffect.PushRight;
                case TranslationEffect.PushRight:
                    return TranslationEffect.PushLeft;
                case TranslationEffect.PushUp:
                    return TranslationEffect.PushDown;
                case TranslationEffect.PushDown:
                    return TranslationEffect.PushUp;
            }
            return Effect;
        }
    }

    #region TranslationExtensions

    public static class TranslationExtensions
    {
        public static IHandler Translate(this IHandler handler,
            TranslationEffect effect, TimeSpan? duration = null)
        {
            return new RegionOptions
            {
                Animation = new Translation(effect)
                {
                    Duration = duration
                }
            }.Decorate(handler);
        }

        public static IHandler SlideLeft(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.SlideLeft, duration);
        }

        public static IHandler SlideRight(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.SlideRight, duration);
        }

        public static IHandler SlideDown(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.SlideDown, duration);
        }

        public static IHandler SlideUp(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.SlideUp, duration);
        }

        public static IHandler PushLeft(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.PushLeft, duration);
        }

        public static IHandler PushRight(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.PushRight, duration);
        }

        public static IHandler PushDown(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.PushDown, duration);
        }

        public static IHandler PushUp(
            this IHandler handler, TimeSpan? duration = null)
        {
            return handler.Translate(TranslationEffect.PushUp, duration);
        }
    }

    #endregion
}
