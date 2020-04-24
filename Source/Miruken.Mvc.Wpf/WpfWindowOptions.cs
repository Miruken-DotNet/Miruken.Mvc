namespace Miruken.Mvc.Wpf
{
    using System.Linq;
    using System.Windows;
    using Callback;
    using Options;
    using WpfScreenHelper;

    public class WpfWindowOptions : Options<WpfWindowOptions>
    {
        public Rect?  Frame  { get; set; }
        public Screen Screen { get; set; }

        public override void MergeInto(WpfWindowOptions other)
        {
            if (Frame.HasValue && !other.Frame.HasValue)
                other.Frame = Frame;

            if (Screen != null && other.Screen == null)
                other.Screen = Screen;
        }

        public Rect? GetFrame(
            ScreenFill? screenFill = null,
            Rect?       hint       = null)
        {
            if (Frame.HasValue) return Frame.Value;
            var screen = Screen ?? Screen.PrimaryScreen;
            if (screen == null || screenFill == null) return null;
            var frame = screen.WorkingArea;
            switch (screenFill)
            {
                case ScreenFill.FullScreen:
                    return frame;
                case ScreenFill.VirtualScreen:
                    return CalculateVirtualFrame();
                case ScreenFill.CenterScreen:
                    return hint.HasValue
                         ? Center(frame, hint.Value.Size)
                         : (Rect?)null;
                case ScreenFill.CenterVirtualScreen:
                    var virtualFrame = CalculateVirtualFrame();
                    return hint.HasValue
                         ? Center(virtualFrame, hint.Value.Size)
                         : virtualFrame;
                case ScreenFill.SplitLeft:
                    return new Rect(frame.Left, frame.Top, frame.Width / 2, frame.Height);
                case ScreenFill.SplitRight:
                    return new Rect(frame.Width / 2, frame.Top, frame.Width / 2, frame.Height);
                case ScreenFill.SplitTop:
                    return new Rect(frame.Left, frame.Top, frame.Width, frame.Height / 2);
                case ScreenFill.SplitBottom:
                    return new Rect(frame.Left, frame.Top / 2, frame.Width, frame.Height / 2);
                default:
                    return null;
            }
        }

        private static Rect CalculateVirtualFrame()
        {
            return Screen.AllScreens.Aggregate(new Rect(),
                (cur, scr) => Rect.Union(cur, scr.Bounds));
        }

        private static Rect Center(Rect owner, Size size)
        {
            var location = new Point((owner.Width - size.Width) / 2,
                                     (owner.Height - size.Height) / 2);
            return new Rect(location, size);
        }
    }
}
