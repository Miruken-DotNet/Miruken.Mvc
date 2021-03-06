﻿namespace Miruken.Mvc.Animation
{
    using System;
    using Infrastructure;

    public abstract class Animation : IAnimation
    {
        public Mode?     Mode     { get; set; }
        public TimeSpan? Duration { get; set; }
        public Fade      Fade     { get; set; }

        public TypeKeyedCollection<object> Behaviors { get; }
            = new TypeKeyedCollection<object>();

        public virtual IAnimation Merge(IAnimation other)
        {
            Fade = Fade ?? other as Fade;
            foreach (var behavior in other.Behaviors)
            {
                if (!Behaviors.Contains(behavior.GetType()))
                    Behaviors.Add(behavior);
            }
            return this;
        }
    }
}
