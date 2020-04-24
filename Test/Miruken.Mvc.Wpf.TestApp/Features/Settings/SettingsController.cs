namespace Miruken.Mvc.Wpf.TestApp.Features.Settings
{
    using About;
    using Mvc;
    using Mvc.Animation;

    public class SettingsController : Controller
    {
        public string Name { get; set; }

        public void Configure(string name)
        {
            Name = name;
            Show<Settings>();
        }

        public void About()
        {
            Push<AboutController>(IO.OpenPortal(), ctrl => ctrl.About());
        }

        public void Back()
        {
            GoBack(IO
                .Zoom()
                .Fade()
                .RollIn(Origin.BottomLeft));
        }
    }
}
