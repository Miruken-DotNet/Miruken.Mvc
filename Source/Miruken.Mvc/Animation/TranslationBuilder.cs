namespace Miruken.Mvc.Animation
{
    using System;

    public class TranslationBuilder
    {
        private TranslationEffect _effect;
        private TimeSpan? _duration;

        public Translation Translation => new Translation(_effect)
        {
            Duration = _duration
        };

        public TranslationBuilder Duration(TimeSpan duration)
        {
            _duration = duration;
            return this;
        }

        public TranslationBuilder Effect(TranslationEffect effect)
        {
            _effect = effect;
            return this;
        }

        public DirectionBuilder Push => new PushSlideBuilder(true, this);
        public DirectionBuilder Slide => new PushSlideBuilder(false, this);
    }

    #region Direction

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public abstract class DirectionBuilder
    {
        private readonly TranslationBuilder _animation;

        protected DirectionBuilder(TranslationBuilder animation)
        {
            _animation = animation;
        }

        public TranslationBuilder Left()
        {
            Apply(Direction.Left, _animation);
            return _animation;
        }

        public TranslationBuilder Right()
        {
            Apply(Direction.Right, _animation);
            return _animation;
        }

        public TranslationBuilder Up()
        {
            Apply(Direction.Up, _animation);
            return _animation;
        }

        public TranslationBuilder Down()
        {
            Apply(Direction.Down, _animation);
            return _animation;
        }

        protected abstract TranslationBuilder Apply
            (Direction direction, TranslationBuilder animation);
    }

    #endregion

    #region Push or Slide

    public class PushSlideBuilder : DirectionBuilder
    {
        private readonly bool _push;

        public PushSlideBuilder(bool push, TranslationBuilder animation)
            : base(animation)
        {
            _push  = push;
        }

        protected override TranslationBuilder Apply(
            Direction direction, TranslationBuilder animation)
        {
            switch (direction)
            {
                case Direction.Left:
                    return animation.Effect(
                        _push ? TranslationEffect.PushLeft : TranslationEffect.SlideLeft);
                case Direction.Right:
                    return animation.Effect(
                        _push ? TranslationEffect.PushRight : TranslationEffect.SlideRight);
                case Direction.Up:
                    return animation.Effect(
                        _push ? TranslationEffect.PushUp : TranslationEffect.SlideUp);
                case Direction.Down:
                    return animation.Effect(
                        _push ? TranslationEffect.PushDown : TranslationEffect.SlideDown);
            }
            return animation;
        }
    }

    #endregion
}
