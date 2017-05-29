namespace Miruken.Mvc.Wpf
{
    using System.Drawing;
    using System.Windows;

    public static class WindowExtensions
    {
        public static Rectangle? GetFrame(this Window window)
        {
            if (double.IsNaN(window.Left)  || double.IsNaN(window.Top) ||
                double.IsNaN(window.Width) || double.IsNaN(window.Height))
            return null;
                
            return new Rectangle(
                (int)window.Left,
                (int)window.Top,
                (int)window.Width,
                (int)window.Height);
        }

        public static void SetFrame(this Window window, Rectangle frame)
        {
            window.Left   = frame.Left;
            window.Top    = frame.Top;
            window.Width  = frame.Width;
            window.Height = frame.Height;
        }
    }
}
