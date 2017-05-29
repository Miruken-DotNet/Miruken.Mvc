namespace MajorLeagueMiruken.Console.Features.Player
{
    using Api;
    using Miruken.Concurrency;
    using Miruken.Mvc;
    using Team;

    public class EditPlayerController : Controller
    {
        public Player Player { get; set; }
        public Team[] Teams  { get; set; }

        public Promise ShowEditPlayer(int id)
        {

            return Promise.All(new Promise[]
            {
                P<IPlayer>(IO).Player(id).Then((player, s) =>
                {
                    Player = player;
                }),
                P<ITeam>(IO).Teams().Then((teams, s) =>
                {
                    Teams = teams;
                })
            }).Then((a,b) =>
            {
                EditPlayerView view = null;
                Show<EditPlayerView>(v => view = v);
                view?.CompleteForm();
            });
        }

        public void UpdatePlayer()
        {
            P<IPlayer>(IO).UpdatePlayer(Player).Then((player, s) =>
            {
                Next<PlayersController>(IO).ShowPlayers();
            });
        }
    }
}
