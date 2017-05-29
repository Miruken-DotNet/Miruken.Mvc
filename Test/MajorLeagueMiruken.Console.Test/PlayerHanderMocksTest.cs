namespace MajorLeagueMiruken.Console.Test
{
    using System.Threading.Tasks;
    using Api;
    using Features.Env;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PlayerHanderMocksTest
    {
        HandlerMocks mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new HandlerMocks();
        }

        [TestMethod]
        public async Task PlayersTest()
        {
            await mock.Players().Then((players, s) =>
            {
                Assert.AreEqual(6, players.Length);
            });
        }

        [TestMethod]
        public async Task PlayerTest()
        {
            await mock.Player(7).Then((player, s) =>
            {
                Assert.IsNotNull(player);
                Assert.AreEqual(7, player.Id);
            });
        }

        [TestMethod]
        public async Task CreatePlayerTest()
        {
            await mock.CreatePlayer(new Player()).Then((player, s) =>
            {
                Assert.AreEqual(13, player.Id);
            });
        }

        [TestMethod]
        public async Task DeletePlayerTest()
        {
            await mock.DeletePlayer(new Player {Id = 7}).Then((item, x) =>
            {
                return mock.Players().Then((players, y) =>
                {
                     Assert.AreEqual(5, players.Length);
                });
            });
        }

        [TestMethod]
        public async Task UpdatePlayerTest()
        {
            await mock.Player(7).Then((player, x) =>
            {
                player.FirstName = "FooBar";
                return mock.UpdatePlayer(player).Then((playerResult, y) =>
                {
                     Assert.AreEqual("FooBar", playerResult.FirstName);
                });
            });
        }
    }
}
