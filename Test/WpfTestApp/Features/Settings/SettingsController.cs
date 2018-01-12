namespace WpfTestApp.Features.Settings
{
    using Miruken.Mvc;

    public class SettingsController : Controller
    {
        public string Name { get; set; }

        public void Configure(string name)
        {
            Name = name;
            Show<Settings>();
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
