namespace Miruken.Mvc.Animation
{
    using System;

    public abstract class Animation : IAnimation
    {
        public TimeSpan? Duration { get; set; }

        public abstract IAnimation CreateInverse();
    }
}
