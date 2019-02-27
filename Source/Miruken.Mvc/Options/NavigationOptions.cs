namespace Miruken.Mvc.Options
{
    using Animation;
    using Callback;

    public class NavigationOptions : Options<NavigationOptions>
    {
        public RegionOptions Region    { get; set; }
        public WindowOptions Window    { get; set; }
        public IAnimation    Animation { get; set; }
        public bool?         NoBack    { get; set; }
        public bool?         GoBack    { get; set; }

        public override void MergeInto(NavigationOptions other)
        {
            if (other == null) return;

            if (NoBack.HasValue && !other.NoBack.HasValue)
                other.NoBack = NoBack;

            if (GoBack.HasValue && !other.GoBack.HasValue)
                other.GoBack = NoBack;

            if (Region != null)
            {
                var r = other.Region ?? new RegionOptions();
                Region.MergeInto(r);
                if (other.Region == null)
                    other.Region = r;
            }

            if (Window != null)
            {
                var w = other.Window ?? new WindowOptions();
                Window.MergeInto(w);
                if (other.Window == null)
                    other.Window = w;
            }

            if (Animation != null)
            {
                other.Animation = other.Animation?.Merge(Animation)
                                  ?? Animation;
            }
        }
    }

    public static class NavigationOptionsExtensions
    {
        public static IHandler NoBack(this IHandler handler)
        {
            return new NavigationOptions
            {
                NoBack = true
            }.Decorate(handler);
        }

        public static IHandler NavBack(this IHandler handler)
        {
            return new NavigationOptions
            {
                GoBack = true
            }.Decorate(handler);
        }

        public static IHandler NavigationOptions(
            this IHandler handler, NavigationOptions options)
        {
            return options.Decorate(handler);
        }
    }
}
