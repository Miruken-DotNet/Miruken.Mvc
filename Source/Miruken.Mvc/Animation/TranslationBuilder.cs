namespace Miruken.Mvc.Animation
{
    public class TranslationBuilder
    {
        public TranslationBuilder()
        {
            Translation = new Translation();
        }

        public Translation Translation { get; }

        public TranslationBuilder Duration(double duration)
        {
            Translation.Duration = duration;
            return this;
        }

        public TranslationBuilder Effect(TranslationEffect effect)
        {
            Translation.Effect = effect;
            return this;
        }

        public DirectionBuilder Push    => new PushMoveBuilder(true, this);
        public DirectionBuilder Move    => new PushMoveBuilder(false, this);
        public DirectionBuilder Cover   => new CoverUncoverBuilder(true, this);
        public DirectionBuilder Uncover => new CoverUncoverBuilder(false, this);
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

    #region Push or Move

    public class PushMoveBuilder : DirectionBuilder
    {
        private readonly bool _push;

        public PushMoveBuilder(bool push, TranslationBuilder animation)
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
                        _push ? TranslationEffect.PushLeft : TranslationEffect.MoveLeft);
                case Direction.Right:
                    return animation.Effect(
                        _push ? TranslationEffect.PushRight : TranslationEffect.MoveRight);
                case Direction.Up:
                    return animation.Effect(
                        _push ? TranslationEffect.PushUp : TranslationEffect.MoveUp);
                case Direction.Down:
                    return animation.Effect(
                        _push ? TranslationEffect.PushDown : TranslationEffect.MoveDown);
            }
            return animation;
        }
    }

    #endregion

    #region Cover or Uncover

    public class CoverUncoverBuilder : DirectionBuilder
    {
        private readonly bool _cover;

        public CoverUncoverBuilder(bool cover, TranslationBuilder animation)
            : base(animation)
        {
            _cover  = cover;
        }

        protected override TranslationBuilder Apply(
            Direction direction, TranslationBuilder animation)
        {
            switch (direction)
            {
                case Direction.Left:
                    return animation.Effect(
                        _cover ? TranslationEffect.CoverLeft : TranslationEffect.UncoverLeft);
                case Direction.Right:
                    return animation.Effect(
                        _cover ? TranslationEffect.CoverRight : TranslationEffect.UncoverRight);
                case Direction.Up:
                    return animation.Effect(
                        _cover ? TranslationEffect.CoverUp : TranslationEffect.UncoverUp);
                case Direction.Down:
                    return animation.Effect(
                        _cover ? TranslationEffect.CoverDown : TranslationEffect.UncoverDown);
            }
            return animation;
        }
    }

    #endregion
}
