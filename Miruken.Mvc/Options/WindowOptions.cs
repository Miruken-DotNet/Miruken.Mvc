namespace Miruken.Mvc.Options
{
    using System;
    using System.Windows.Forms;
    using Callback;

    public enum ScreenFill
    {
        FullScreen,
        VirtualScreen,
        SplitLeft,
        SplitRight,
        SplitTop,
        SplitBottom
    }

    public class WindowOptions : CallbackOptions<WindowOptions>
    {
        public bool?       NewWindow  { get; set; }
        public bool?       Modal      { get; set; }
        public Screen      Screen     { get; set; }
        public ScreenFill? FillScreen { get; set; }

        public override void MergeInto(WindowOptions other)
        {
            if (NewWindow.HasValue && !other.NewWindow.HasValue)
                other.NewWindow = NewWindow;

            if (Modal.HasValue && !other.Modal.HasValue)
                other.Modal = Modal;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;

            if (FillScreen.HasValue && !other.FillScreen.HasValue)
                other.FillScreen = FillScreen;
        }
    }

    public static class WindowOptionsExtensions
    {
        public static IHandler NewWindow(this IHandler handler, Screen screen = null)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    NewWindow = true,
                    Screen    = screen
                }
            }.Decorate(handler);
        }

        public static IHandler NewWindow(this IHandler handler, Action<ScreenBuilder> build)
        {
            var window = new WindowOptions { NewWindow = true };
            if (build != null)
            {
                var builder = new ScreenBuilder(window);
                build(builder);
            }
            return new RegionOptions {  Window = window }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Screen screen = null)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    NewWindow = true,
                    Modal     = true,
                    Screen    = screen
                }
            }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Action<ScreenBuilder> build)
        {
            var window = new WindowOptions { NewWindow = true, Modal = true };
            if (build != null)
            {
                var builder = new ScreenBuilder(window);
                build(builder);
            }
            return new RegionOptions { Window = window }.Decorate(handler);
        }
    }
}
