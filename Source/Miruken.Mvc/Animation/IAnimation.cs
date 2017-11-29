namespace Miruken.Mvc.Animation
{
    using System;

    public interface IAnimation
    {
        TimeSpan? Duration { get; set; }

        IAnimation CreateInverse();
    }
}
