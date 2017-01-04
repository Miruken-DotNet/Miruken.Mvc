namespace Miruken.Mvc.Options
{
    using System.Windows.Forms;
    using Callback;

    public class WindowOptions : CallbackOptions<WindowOptions>
    {
        public bool?  PrimaryScreen   { get; set; }

        public bool?  SecondaryScreen { get; set; }

        public Screen Screen          { get; set; }

        public bool?  Modal           { get; set; }

        public override void MergeInto(WindowOptions other)
        {
            if (PrimaryScreen.HasValue && !other.PrimaryScreen.HasValue)
                other.PrimaryScreen = PrimaryScreen;

            if (SecondaryScreen.HasValue && !other.SecondaryScreen.HasValue)
                other.SecondaryScreen = SecondaryScreen;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;

            if (Modal.HasValue && !other.Modal.HasValue)
                other.Modal = Modal;
        }
    }

    public static class ScreenOptionsExtensions
    {
        public static IHandler PrimaryScreen(this IHandler handler)
        {
            return new RegionOptions
            {
                Window = new WindowOptions { PrimaryScreen = true }
            }.Decorate(handler);
        }

        public static IHandler SecondaryScreen(this IHandler handler)
        {
            return new RegionOptions
            {
                Window = new WindowOptions { SecondaryScreen = true }
            }.Decorate(handler);
        }

        public static IHandler Screen(this IHandler handler, Screen screen)
        {
            return new RegionOptions
            {
                Window = new WindowOptions { Screen = screen }
            }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, bool modal = true)
        {
            return new RegionOptions
            {
                Window = new WindowOptions { Modal = modal }
            }.Decorate(handler);
        }
    }
}
