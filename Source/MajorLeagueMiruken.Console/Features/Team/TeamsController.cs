namespace MajorLeagueMiruken.Console.Features.Team
{
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Miruken.Concurrency;

    public class TeamsController : FeatureController
    {
        public IEnumerable<Team> Teams { get; set; }

        public Promise ShowTeams()
        {
            return P<ITeam>(IO).Teams().Then((teams, n) =>
            {
                Teams = teams.OrderBy(x => x.Name);
                Show<TeamsView>();
            });
        }
    }
}
