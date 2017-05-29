namespace MajorLeagueMiruken.Console.Features.Team
{
    using Api;
    using Miruken.Concurrency;
    using Miruken.Mvc;

    public class EditTeamController : Controller
    {
        public Team Team { get; set; }

        public Promise ShowEditTeam(int id)
        {
            EditTeamView view = null;

            return P<ITeam>(IO).Team(id).Then((team, s) =>
            {
                Team = team;
                Show<EditTeamView>(v => view = v);
                view?.CompleteForm();
            });
        }

        public Promise UpdateTeam()
        {
            return P<ITeam>(IO).UpdateTeam(Team).Then((team, s) =>
            {
                Next<TeamsController>(IO).ShowTeams();
            });
        }
    }
}
