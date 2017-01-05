namespace Miruken.Mvc.Options
{
    using System;
    using System.Drawing;
    using System.Linq;
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

        public Rectangle? GetWindowFrame()
        {
            var screen = Screen ?? Screen.PrimaryScreen;
            if (screen == null || !FillScreen.HasValue)
                return null;
            var frame = screen.WorkingArea;
            switch (FillScreen)
            {
                case ScreenFill.FullScreen:
                    return frame;
                case ScreenFill.VirtualScreen:
                    return CalculateVirtualScreen();
                case ScreenFill.SplitLeft:
                    return new Rectangle(frame.Left, frame.Top, frame.Width / 2, frame.Height);
                case ScreenFill.SplitRight:
                    return new Rectangle(frame.Width / 2, frame.Top, frame.Width / 2, frame.Height);
                case ScreenFill.SplitTop:
                    return new Rectangle(frame.Left, frame.Top, frame.Width, frame.Height / 2);
                case ScreenFill.SplitBottom:
                    return new Rectangle(frame.Left, frame.Top / 2, frame.Width, frame.Height / 2);
                default:
                    return null;
            }
        }

        private static Rectangle CalculateVirtualScreen()
        {
            var rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            return Screen.AllScreens.Aggregate(rect, (cur, scr) => Rectangle.Union(cur, scr.Bounds));
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
                    NewWindow = true, Screen = screen
                }
            }.Decorate(handler);
        }

        public static IHandler NewWindow(this IHandler handler, Action<ScreenBuilder> build)
        {
            var window = new WindowOptions {NewWindow = true};
            if (build != null)
            {
                var builder = new ScreenBuilder(window);
                build(builder);
            }
            return new RegionOptions {Window = window}.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Screen screen = null)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    NewWindow = true, Modal = true, Screen = screen
                }
            }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Action<ScreenBuilder> build)
        {
            var window = new WindowOptions {NewWindow = true, Modal = true};
            if (build != null)
            {
                var builder = new ScreenBuilder(window);
                build(builder);
            }
            return new RegionOptions {Window = window}.Decorate(handler);
        }
    }
}
