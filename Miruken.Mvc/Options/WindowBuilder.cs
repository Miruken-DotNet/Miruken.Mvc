namespace Miruken.Mvc.Options
{
    using System.Linq;
    using System.Windows.Forms;

    public class WindowBuilder
    {
        public WindowBuilder(WindowOptions options)
        {
            WindowOptions = options;
        }

        public WindowOptions WindowOptions { get; }

        public WindowBuilder Name(string name)
        {
            WindowOptions.Name = name;
            return this;
        }

        public WindowBuilder Title(string title)
        {
            WindowOptions.Title = title;
            return this;
        }

        public WindowBuilder PrimaryScreen()
        {
            WindowOptions.Screen = Screen.PrimaryScreen;
            return this;
        }

        public WindowBuilder SecondaryScreen()
        {
            WindowOptions.Screen = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
            return this;
        }

        public WindowBuilder FullScreen()
        {
            WindowOptions.FillScreen = ScreenFill.FullScreen;
            return this;
        }

        public WindowBuilder VirtualScreen()
        {
            WindowOptions.FillScreen = ScreenFill.VirtualScreen;
            return this;
        }

        public WindowBuilder SplitLeft()
        {
            WindowOptions.FillScreen = ScreenFill.SplitLeft;
            return this;
        }

        public WindowBuilder SplitRight()
        {
            WindowOptions.FillScreen = ScreenFill.SplitRight;
            return this;
        }

        public WindowBuilder SplitTop()
        {
            WindowOptions.FillScreen = ScreenFill.SplitTop;
            return this;
        }

        public WindowBuilder SplitBottom()
        {
            WindowOptions.FillScreen = ScreenFill.SplitBottom;
            return this;
        }

        public WindowBuilder FillScreen(ScreenFill fillScreen)
        {
            WindowOptions.FillScreen = fillScreen;
            return this;
        }

        public WindowBuilder Readonly()
        {
            WindowOptions.Readonly = true;
            return this;
        }

        public WindowBuilder HideCursor()
        {
            WindowOptions.HideCursor = true;
            return this;
        }
    }
}
