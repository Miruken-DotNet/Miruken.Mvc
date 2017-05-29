namespace MajorLeagueMiruken.Console.Features.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Miruken.Concurrency;

    public class PlayersController : FeatureController
    {
        public IEnumerable<Player> Players;

        public Promise ShowPlayers()
        {
            return P<IPlayer>(IO).Players().Then((players, s) =>
            {
                Players = players.OrderBy(x => x.FirstName);
                Show<PlayersView>();
            });
        }
    }
}
