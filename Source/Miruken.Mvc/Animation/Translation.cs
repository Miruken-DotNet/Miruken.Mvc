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

    public class Translation : IAnimation
    {
        public TranslationEffect Effect   { get; }
        public double?           Duration { get; set; }

        public Translation(TranslationEffect effect)
        {
            Effect = effect;
        }

        public IAnimation CreateInverse()
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
        public static IHandler Translate(
            this IHandler handler, TranslationEffect effect)
        {
            return new RegionOptions
            {
                Animation = new Translation(effect)
            }.Decorate(handler);
        }

        public static IHandler Translate(this IHandler handler,
            TranslationEffect effect, double duration)
        {
            return new RegionOptions
            {
                Animation = new Translation(effect)
                {
                    Duration = duration
                }
            }.Decorate(handler);
        }

        public static IHandler Translate(
             this IHandler handler, Action<TranslationBuilder> build)
        {
            if (handler == null || build == null)
                return handler;
            var builder = new TranslationBuilder();
            build(builder);
            return new RegionOptions
            {
                Animation = builder.Translation
            }.Decorate(handler);
        }

        public static IHandler SlideLeft(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.SlideLeft)
            }.Decorate(handler);
        }

        public static IHandler SlideRight(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.SlideRight)
            }.Decorate(handler);
        }

        public static IHandler SlideDown(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.SlideDown)
            }.Decorate(handler);
        }

        public static IHandler SlideUp(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.SlideUp)
            }.Decorate(handler);
        }

        public static IHandler PushLeft(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.PushLeft)
            }.Decorate(handler);
        }

        public static IHandler PushRight(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.PushRight)
            }.Decorate(handler);
        }

        public static IHandler PushDown(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.PushDown)
            }.Decorate(handler);
        }

        public static IHandler PushUp(this IHandler handler)
        {
            return new RegionOptions
            {
                Animation = new Translation(TranslationEffect.PushUp)
            }.Decorate(handler);
        }
    }

    #endregion
}
