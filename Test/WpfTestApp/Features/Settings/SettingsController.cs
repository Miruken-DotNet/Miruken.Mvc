namespace WpfTestApp.Features.Settings
{
    using Miruken.Mvc;

    public class SettingsController : Controller
    {
        public void Configure()
        {
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
