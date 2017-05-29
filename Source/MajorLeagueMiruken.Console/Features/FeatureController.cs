namespace MajorLeagueMiruken.Console.Features
{
    using Miruken.Concurrency;
    using Miruken.Mvc;
    using Player;
    using Team;

    public class FeatureController : Controller
    {
        public Promise GoToTeam(Api.Team team)
        {
            return Next<TeamController>().ShowTeam(team.Id);
        }

        public Promise GoToCreateTeam()
        {
            return Next<CreateTeamController>().ShowCreateTeam();
        }

        public Promise GoToPlayer(Api.Player player)
        {
            return Next<PlayerController>().ShowPlayer(player.Id);
        }

        public Promise GoToCreatePlayer()
        {
            return Next<CreatePlayerController>().ShowCreatePlayer();
        }
    }
}