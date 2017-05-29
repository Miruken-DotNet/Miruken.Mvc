namespace MajorLeagueMiruken.Console.Features.Player
{
    using Api;
    using Miruken.Concurrency;
    using Miruken.Mvc;
    using Team;

    public class CreatePlayerController : Controller
    {
        public Player Player { get; set; } = new Player();
        public Team[] Teams  { get; set; }

        public Promise ShowCreatePlayer()
        {
            return P<ITeam>(IO).Teams().Then((teams, s) =>
            {
                Teams = teams;
                CreatePlayerView view = null;
                Show<CreatePlayerView>(v => view = v);
                view?.CompleteForm();
            });
        }

        public Promise CreatePlayer()
        {
            return P<IPlayer>(IO).CreatePlayer(Player).Then((player, s) =>
            {
                Next<PlayerController>().ShowPlayer(player.Id);
            });
        }
    }
}
