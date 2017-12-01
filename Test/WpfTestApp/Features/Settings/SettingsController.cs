namespace WpfTestApp.Features.Settings
{
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;

    public class SettingsController : Controller
    {
        public void Configure()
        {
            Show<Settings>();
        }

        public void Back()
        {
            GoBack(IO.Fade().RollIn(Position.BottomRight));
        }
    }
}
