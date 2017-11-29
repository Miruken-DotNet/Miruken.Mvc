namespace Miruken.Mvc.Animation
{
    using System;
    using Infrastructure;

    public interface IAnimation
    {
        TimeSpan? Duration { get; set; }

        TypeKeyedCollection<object> Behaviors { get; }

        IAnimation CreateInverse();

        IAnimation Merge(IAnimation other);
    }
}
