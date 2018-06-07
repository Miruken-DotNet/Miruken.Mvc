namespace WpfTestApp.Features.Settings
{
    using About;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;

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
            Push<AboutController>(IO.OpenPortal()).About();
        }

        public void Back()
        {
            EndContext();
            /*
            GoBack(IO
                .Zoom()
                .Fade()
                .RollIn(Position.BottomLeft));
                */
        }
    }
}
