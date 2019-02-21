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
        CenterScreen,
        CenterVirtualScreen,
        CenterParent,
        SplitLeft,
        SplitRight,
        SplitTop,
        SplitBottom
    }

    public class WindowOptions : Options<WindowOptions>
    {
        public string      Name       { get; set; }
        public string      Title      { get; set; }
        public bool?       Modal      { get; set; }
        public bool?       Standalone { get; set; }
        public bool?       Readonly   { get; set; }
        public bool?       HideCursor { get; set; }
        public Screen      Screen     { get; set; }
        public ScreenFill? FillScreen { get; set; }
        public Rectangle?  Frame      { get; set; }
        public Type        WindowType { get; set; }

        public override void MergeInto(WindowOptions other)
        {
            if (other == null) return;

            if (Name != null && other.Name == null)
                other.Name = Name;

            if (Title != null && other.Title == null)
                other.Title = Title;

            if (Modal.HasValue && !other.Modal.HasValue)
                other.Modal = Modal;

            if (Standalone.HasValue && !other.Standalone.HasValue)
                other.Standalone = Standalone;

            if (Readonly.HasValue && !other.Readonly.HasValue)
                other.Readonly = Readonly;

            if (HideCursor.HasValue && !other.HideCursor.HasValue)
                other.HideCursor = HideCursor;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;

            if (FillScreen.HasValue && !other.FillScreen.HasValue)
                other.FillScreen = FillScreen;

            if (Frame.HasValue && !other.Frame.HasValue)
                other.Frame = Frame;

            if (WindowType != null && other.WindowType == null)
                other.WindowType = WindowType;
        }

        public Rectangle? GetFrame(Rectangle? hint = null)
        {
            if (Frame.HasValue) return Frame.Value;
            var screen = Screen ?? Screen.PrimaryScreen;
            if (screen == null || !FillScreen.HasValue)
                return null;
            var frame = screen.WorkingArea;
            switch (FillScreen)
            {
                case ScreenFill.FullScreen:
                    return frame;
                case ScreenFill.VirtualScreen:
                    return CalculateVirtualFrame();
                case ScreenFill.CenterScreen:
                    return hint.HasValue
                         ? Center(frame, hint.Value.Size)
                         : (Rectangle?)null;
                case ScreenFill.CenterVirtualScreen:
                    var virtualFrame = CalculateVirtualFrame();
                    return hint.HasValue
                         ? Center(virtualFrame, hint.Value.Size)
                         : virtualFrame;
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

        private static Rectangle CalculateVirtualFrame()
        {
            var rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            return Screen.AllScreens.Aggregate(rect, (cur, scr) => Rectangle.Union(cur, scr.Bounds));
        }

        private static Rectangle Center(Rectangle owner, Size size)
        {
            var location = new Point((owner.Width  - size.Width)  / 2,
                                     (owner.Height - size.Height) / 2);
            return new Rectangle(location, size);
        }
    }

    public static class WindowOptionsExtensions
    {
        public static IHandler NewWindow(this IHandler handler, WindowOptions options = null)
        {
            return new NavigationOptions
            {
                Window = options ?? new WindowOptions()
            }.Decorate(handler);
        }

        public static IHandler NewWindow(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions();
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, WindowOptions window = null)
        {
            window = window ?? new WindowOptions();
            window.Modal = true;
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Modal(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions { Modal = true };
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Standalone(this IHandler handler, WindowOptions window = null)
        {
            window = window ?? new WindowOptions();
            window.Standalone = true;
            return new NavigationOptions { Window = window }.Decorate(handler);
        }

        public static IHandler Standalone(this IHandler handler, Action<WindowBuilder> build)
        {
            var window = new WindowOptions { Standalone = true };
            if (build != null)
            {
                var builder = new WindowBuilder(window);
                build(builder);
            }
            return new NavigationOptions { Window = window }.Decorate(handler);
        }
    }
}
