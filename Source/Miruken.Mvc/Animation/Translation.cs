namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public enum TranslationEffect
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        PushLeft,
        PushRight,
        PushUp,
        PushDown,
        CoverLeft,
        CoverRight,
        CoverUp,
        CoverDown,
        UncoverLeft,
        UncoverRight,
        UncoverUp,
        UncoverDown
    }

    public class Translation
    {
        public TranslationEffect Effect   { get; set; }
        public double?           Duration { get; set; }

        public TranslationEffect InverseEffect
        {
            get
            {
                switch (Effect)
                {
                    case TranslationEffect.MoveLeft:
                        return TranslationEffect.MoveRight;
                    case TranslationEffect.MoveRight:
                        return TranslationEffect.MoveLeft;
                    case TranslationEffect.MoveUp:
                        return TranslationEffect.MoveDown;
                    case TranslationEffect.MoveDown:
                        return TranslationEffect.MoveUp;
                    case TranslationEffect.PushLeft:
                        return TranslationEffect.PushRight;
                    case TranslationEffect.PushRight:
                        return TranslationEffect.PushLeft;
                    case TranslationEffect.PushUp:
                        return TranslationEffect.PushDown;
                    case TranslationEffect.PushDown:
                        return TranslationEffect.PushUp;
                    case TranslationEffect.CoverLeft:
                        return TranslationEffect.UncoverRight;
                    case TranslationEffect.CoverRight:
                        return TranslationEffect.UncoverLeft;
                    case TranslationEffect.CoverUp:
                        return TranslationEffect.UncoverDown;
                    case TranslationEffect.CoverDown:
                        return TranslationEffect.UncoverUp;
                    case TranslationEffect.UncoverLeft:
                        return TranslationEffect.CoverRight;
                    case TranslationEffect.UncoverRight:
                        return TranslationEffect.CoverLeft;
                    case TranslationEffect.UncoverUp:
                        return TranslationEffect.CoverDown;
                    case TranslationEffect.UncoverDown:
                        return TranslationEffect.CoverUp;
                }
                return Effect;
            }
        }
    }

    public static class AnimationOptionsExtensions
    {
        public static IHandler Translate(
            this IHandler handler, TranslationEffect effect)
        {
            return new RegionOptions
            {
                Animation = new Translation
                {
                    Effect = effect
                }
            }.Decorate(handler);
        }

        public static IHandler Translate(this IHandler handler,
            TranslationEffect effect, double duration)
        {
            return new RegionOptions
            {
                Animation = new Translation
                {
                    Effect   = effect,
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
    }
}
