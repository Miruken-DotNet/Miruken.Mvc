namespace Miruken.Mvc.Options
{
    using System.Windows.Forms;
    using Callback;

    public class ScreenOptions : CallbackOptions<ScreenOptions>
    {
        public bool?  Primary   { get; set; }

        public bool?  Secondary { get; set; }

        public Screen Screen    { get; set; }

        public override void MergeInto(ScreenOptions other)
        {
            if (Primary.HasValue && !other.Primary.HasValue)
                other.Primary = Primary;

            if (Secondary.HasValue && !other.Secondary.HasValue)
                other.Secondary = Secondary;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;
        }
    }

    public static class ScreenOptionsExtensions
    {
        public static IHandler PrimaryScreen(this IHandler handler)
        {
            return new RegionOptions
            {
                Screen = new ScreenOptions {Primary = true}
            }.Decorate(handler);
        }

        public static IHandler SecondaryScreen(this IHandler handler)
        {
            return new RegionOptions
            {
                Screen = new ScreenOptions {Secondary = true}
            }.Decorate(handler);
        }

        public static IHandler Screen(this IHandler handler, Screen screen)
        {
            return new RegionOptions
            {
                Screen = new ScreenOptions {Screen = screen}
            }.Decorate(handler);
        }
    }
}
