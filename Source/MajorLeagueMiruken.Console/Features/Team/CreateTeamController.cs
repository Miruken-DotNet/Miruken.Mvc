namespace MajorLeagueMiruken.Console.Features.Team
{
    using Api;
    using Miruken.Concurrency;
    using Miruken.Mvc;

    public class CreateTeamController : Controller
    {
        public Team Team { get; set; } = new Team
        {
            Manager = new Person(),
            Coach   = new Person()
        };

        public Promise ShowCreateTeam()
        {
            CreateTeamView view = null;
            Show<CreateTeamView>(v => view = v);
            return view?.CompleteForm();
        }

        public Promise CreateTeam()
        {
            return P<ITeam>(IO).CreateTeam(Team).Then((team, s) =>
                Next<TeamController>().ShowTeam(team.Id));
        }

        public void Cancel()
        {

        }
    }

}
