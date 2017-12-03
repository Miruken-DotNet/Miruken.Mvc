namespace Miruken.Mvc.Animation
{
    using System;
    using Infrastructure;

    public interface IAnimation
    {
        Mode?     Mode     { get; set; }
        TimeSpan? Duration { get; set; }
        Fade      Fade     { get; set; }

        TypeKeyedCollection<object> Behaviors { get; }

        IAnimation Merge(IAnimation other);
    }
}
