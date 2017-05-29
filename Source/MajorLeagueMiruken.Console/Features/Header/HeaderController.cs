namespace MajorLeagueMiruken.Console.Features.Header
{
    using System.Threading.Tasks;
    using Miruken.Concurrency;
    using Miruken.Context;
    using Miruken.Mvc;
    using Player;
    using Team;

    public class HeaderController : Controller
    {
        private IContext _body;

        public void ShowHeader(IContext body)
        {
            _body = body;
            Show<HeaderView>();
        }

        public Promise GoToTeams()
        {
            return Next<TeamsController>(_body).ShowTeams();
        }

        public Promise GoToPlayers()
        {
            return Next<PlayersController>(_body).ShowPlayers();
        }
    }
}
