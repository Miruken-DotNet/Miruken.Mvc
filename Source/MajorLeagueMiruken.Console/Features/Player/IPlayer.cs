namespace MajorLeagueMiruken.Console.Features.Player
{
    using Miruken.Concurrency;
    using Api;
    using Miruken.Callback;

    public interface IPlayer : IResolving
    {
        Promise<Player>   CreatePlayer(Player player);
        Promise           DeletePlayer(Player player);
        Promise<Player>   Player(int id);
        Promise<Player[]> Players();
        Promise<Player>   UpdatePlayer(Player player);
    }
}
