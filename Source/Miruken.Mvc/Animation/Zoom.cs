namespace Miruken.Mvc.Animation
{
    using System;
    using Callback;
    using Options;

    public class Zoom : Animation
    {
    }

    public static class ZomeExtensions
    {
        public static IHandler Zoom(
            this IHandler handler, TimeSpan? duration = null)
        {
            return new RegionOptions
            {
                Animation = new Zoom
                {
                    Duration = duration
                }
            }.Decorate(handler);
        }
    }
}
