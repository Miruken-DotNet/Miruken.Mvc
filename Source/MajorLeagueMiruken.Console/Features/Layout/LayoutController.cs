namespace MajorLeagueMiruken.Console.Features.Layout
{
    using Header;
    using Miruken.Mvc;
    using Team;

    public class LayoutController : Controller
    {
        public void ShowLayout()
        {
            Show<LayoutView>(v =>
             {
                 var header = AddRegion(v.Header);
                 var body   = AddRegion(v.Body);

                 Next<HeaderController>(header).ShowHeader(body);
                 Next<TeamsController>(body).ShowTeams();
             });
        }
    }
}