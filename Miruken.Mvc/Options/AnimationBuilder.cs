namespace Miruken.Mvc.Options
{
    public class AnimationBuilder
    {
        public AnimationBuilder()
        {
            AnimationOptions = new AnimationOptions();
        }

        public AnimationOptions AnimationOptions { get; }

        public AnimationBuilder Duration(double duration)
        {
            AnimationOptions.Duration = duration;
            return this;
        }

        public AnimationBuilder Effect(AnimationEffect effect)
        {
            AnimationOptions.Effect = effect;
            return this;
        }

        public DirectionBuilder Push => new PushMoveBuilder(true, this);

        public DirectionBuilder Move => new PushMoveBuilder(false, this);

        public DirectionBuilder Cover => new CoverUncoverBuilder(true, this);

        public DirectionBuilder Uncover => new CoverUncoverBuilder(false, this);

        public AnimationBuilder AlphaBlend()
        {
            return Effect(AnimationEffect.AlphaBlend);
        }

        public AnimationBuilder None()
        {
            return Effect(AnimationEffect.None);
        }
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
        private readonly AnimationBuilder _animation;

        protected DirectionBuilder(AnimationBuilder animation)
        {
            _animation = animation;
        }

        public AnimationBuilder Left()
        {
            Apply(Direction.Left, _animation);
            return _animation;
        }

        public AnimationBuilder Right()
        {
            Apply(Direction.Right, _animation);
            return _animation;
        }

        public AnimationBuilder Up()
        {
            Apply(Direction.Up, _animation);
            return _animation;
        }

        public AnimationBuilder Down()
        {
            Apply(Direction.Down, _animation);
            return _animation;
        }

        protected abstract AnimationBuilder Apply
            (Direction direction, AnimationBuilder animation);
    }

    #endregion

    #region Push or Move

    public class PushMoveBuilder : DirectionBuilder
    {
        private readonly bool _push;

        public PushMoveBuilder(bool push, AnimationBuilder animation)
            : base(animation)
        {
            _push  = push;
        }

        protected override AnimationBuilder Apply(
            Direction direction, AnimationBuilder animation)
        {
            switch (direction)
            {
                case Direction.Left:
                    return animation.Effect(
                        _push ? AnimationEffect.PushLeft : AnimationEffect.MoveLeft);
                case Direction.Right:
                    return animation.Effect(
                        _push ? AnimationEffect.PushRight : AnimationEffect.MoveRight);
                case Direction.Up:
                    return animation.Effect(
                        _push ? AnimationEffect.PushUp : AnimationEffect.MoveUp);
                case Direction.Down:
                    return animation.Effect(
                        _push ? AnimationEffect.PushDown : AnimationEffect.MoveDown);
            }
            return animation;
        }
    }

    #endregion

    #region Cover or Uncover

    public class CoverUncoverBuilder : DirectionBuilder
    {
        private readonly bool _cover;

        public CoverUncoverBuilder(bool cover, AnimationBuilder animation)
            : base(animation)
        {
            _cover  = cover;
        }

        protected override AnimationBuilder Apply(
            Direction direction, AnimationBuilder animation)
        {
            switch (direction)
            {
                case Direction.Left:
                    return animation.Effect(
                        _cover ? AnimationEffect.CoverLeft : AnimationEffect.UncoverLeft);
                case Direction.Right:
                    return animation.Effect(
                        _cover ? AnimationEffect.CoverRight : AnimationEffect.UncoverRight);
                case Direction.Up:
                    return animation.Effect(
                        _cover ? AnimationEffect.CoverUp : AnimationEffect.UncoverUp);
                case Direction.Down:
                    return animation.Effect(
                        _cover ? AnimationEffect.CoverDown : AnimationEffect.UncoverDown);
            }
            return animation;
        }
    }

    #endregion
}
