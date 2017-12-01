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

    public class Translate : Animation
    {
        public Translate(TranslationEffect effect)
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
    }

    #region TranslateExtensions

    public static class TranslateExtensions
    {
        public static IHandler Translate(
            this IHandler handler, Translate translate)
        {
            if (translate == null)
                throw new ArgumentNullException(nameof(translate));
            return new RegionOptions
            {
                Animation = translate
            }.Decorate(handler);
        }

        public static IHandler Translate(this IHandler handler,
            TranslationEffect effect, TimeSpan? duration = null)
        {
            return handler.Translate(new Translate(effect)
            {
                Duration = duration
            });
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
