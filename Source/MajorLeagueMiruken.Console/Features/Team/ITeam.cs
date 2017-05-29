namespace MajorLeagueMiruken.Console.Features.Team
{
    using Api;
    using Miruken.Callback;
    using Miruken.Concurrency;

    public interface ITeam : IResolving
    {
        Promise<Team>   CreateTeam (Team team);
        Promise         DeleteTeam (Team team);
        Promise<Team[]> Teams      ();
        Promise<Team>   Team       (int id);
        Promise<Team>   UpdateTeam (Team team);
        Promise         addPlayers (Player[] players, Team team);
    }
}
