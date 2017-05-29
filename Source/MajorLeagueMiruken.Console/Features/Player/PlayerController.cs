namespace MajorLeagueMiruken.Console.Features.Player
{
    using Api;
    using Miruken.Concurrency;

    public class PlayerController : FeatureController
    {
        public Player Player { get; set; }

        public Promise ShowPlayer(int id)
        {
            return P<IPlayer>().Player(id).Then((player, s) =>
             {
                Player = player;
                Show<PlayerView>();
             });
        }

        public Promise GoToEditPlayer()
        {
            return Next<EditPlayerController>().ShowEditPlayer(Player.Id);
        }
    }
}
