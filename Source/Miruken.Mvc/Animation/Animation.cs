namespace Miruken.Mvc.Animation
{
    using System;
    using Infrastructure;

    public abstract class Animation : IAnimation
    {
        public TimeSpan? Duration { get; set; }

        public TypeKeyedCollection<object> Behaviors { get; }
            = new TypeKeyedCollection<object>();

        public virtual IAnimation Merge(IAnimation other)
        {
            foreach (var behavior in other.Behaviors)
            {
                if (!Behaviors.Contains(behavior.GetType()))
                    Behaviors.Add(behavior);
            }
            return this;
        }

        public virtual IAnimation CreateInverse()
        {
            return null;
        }
    }
}
