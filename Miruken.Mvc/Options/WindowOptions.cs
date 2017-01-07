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
        CenterOwner,
        SplitLeft,
        SplitRight,
        SplitTop,
        SplitBottom
    }

    public class WindowOptions : CallbackOptions<WindowOptions>
    {
        public bool?       NewWindow  { get; set; }
        public bool?       Modal      { get; set; }
        public bool?       Standalone { get; set; }
        public Screen      Screen     { get; set; }
        public ScreenFill? FillScreen { get; set; }
        public Rectangle?  Frame      { get; set; }

        public override void MergeInto(WindowOptions other)
        {
            if (NewWindow.HasValue && !other.NewWindow.HasValue)
                other.NewWindow = NewWindow;

            if (Modal.HasValue && !other.Modal.HasValue)
                other.Modal = Modal;

            if (Standalone.HasValue && !other.Standalone.HasValue)
                other.Standalone = Standalone;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;

            if (FillScreen.HasValue && !other.FillScreen.HasValue)
                other.FillScreen = FillScreen;

            if (Frame.HasValue && !other.Frame.HasValue)
                other.Frame = Frame;
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
                         : frame;
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

        public static IHandler NewWindow(this IHandler handler, Rectangle frame)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    NewWindow = true,
                    Frame = frame
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

        public static IHandler Modal(this IHandler handler, Rectangle frame)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    NewWindow = true,
                    Modal     = true,
                    Frame     = frame
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

        public static IHandler Standalone(this IHandler handler)
        {
            return new RegionOptions
            {
                Window = new WindowOptions
                {
                    Standalone = true
                }
            }.Decorate(handler);
        }
    }
}
