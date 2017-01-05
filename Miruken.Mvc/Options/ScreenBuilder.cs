namespace Miruken.Mvc.Options
{
    using System.Linq;
    using System.Windows.Forms;

    public class ScreenBuilder
    {
        public ScreenBuilder(WindowOptions options)
        {
            WindowOptions = options;
        }

        public WindowOptions WindowOptions { get; }

        public ScreenBuilder Primary()
        {
            WindowOptions.Screen = Screen.PrimaryScreen;
            return this;
        }

        public ScreenBuilder Secondary()
        {
            WindowOptions.Screen = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
            return this;
        }

        public ScreenBuilder FullScreen()
        {
            WindowOptions.FillScreen = ScreenFill.FullScreen;
            return this;
        }

        public ScreenBuilder VirtualScreen()
        {
            WindowOptions.FillScreen = ScreenFill.VirtualScreen;
            return this;
        }

        public ScreenBuilder SplitLeft()
        {
            WindowOptions.FillScreen = ScreenFill.SplitLeft;
            return this;
        }

        public ScreenBuilder SplitRight()
        {
            WindowOptions.FillScreen = ScreenFill.SplitRight;
            return this;
        }

        public ScreenBuilder SplitTop()
        {
            WindowOptions.FillScreen = ScreenFill.SplitTop;
            return this;
        }

        public ScreenBuilder SplitBottom()
        {
            WindowOptions.FillScreen = ScreenFill.SplitBottom;
            return this;
        }

        public ScreenBuilder FillScreen(ScreenFill fillScreen)
        {
            WindowOptions.FillScreen = fillScreen;
            return this;
        }
    }
}
