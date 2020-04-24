namespace Miruken.Mvc.Wpf
{
    using System.Windows;

    public static class WindowExtensions
    {
        public static Rect? GetFrame(this Window window)
        {
            if (double.IsNaN(window.Left)  || double.IsNaN(window.Top) ||
                double.IsNaN(window.Width) || double.IsNaN(window.Height))
                return null;
                
            return new Rect(window.Left, window.Top, window.Width, window.Height);
        }

        public static void SetFrame(this Window window, Rect frame)
        {
            window.Left   = frame.Left;
            window.Top    = frame.Top;
            window.Width  = frame.Width;
            window.Height = frame.Height;
        }
    }
}
