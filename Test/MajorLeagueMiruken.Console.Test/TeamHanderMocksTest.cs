namespace MajorLeagueMiruken.Console.Test
{
    using System.Threading.Tasks;
    using Api;
    using Features.Env;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TeamHanderMocksTest
    {
        HandlerMocks mock;

        [TestInitialize]
        public void TestInitialize()
        {
            mock = new HandlerMocks();
        }

        [TestMethod]
        public async Task TeamsTest()
        {
            await mock.Teams().Then((teams, s) =>
            {
                Assert.AreEqual(6, teams.Length);
            });
        }

        [TestMethod]
        public async Task TeamTest()
        {
            await mock.Team(1).Then((team, s) =>
            {
                Assert.IsNotNull(team);
                Assert.AreEqual(1, team.Id);
            });
        }

        [TestMethod]
        public async Task CreateTeamTest()
        {
            await mock.CreateTeam(new Team()).Then((team, s) =>
            {
                Assert.AreEqual(13, team.Id);
            });
        }

        [TestMethod]
        public async Task DeleteTeamTest()
        {
            await mock.DeleteTeam(new Team {Id = 1}).Then((item, x) =>
            {
                return mock.Teams().Then((teams, y) =>
                {
                     Assert.AreEqual(5, teams.Length);
                });
            });
        }

        [TestMethod]
        public async Task UpdateTeamTest()
        {
            await mock.Team(1).Then((team, x) =>
            {
                team.Name = "FooBar";
                return mock.UpdateTeam(team).Then((teamResult, y) =>
                {
                     Assert.AreEqual("FooBar", teamResult.Name);
                });
            });
        }

        [TestMethod]
        public async Task AddPlayerTest()
        {
            await mock.Team(1)
                .Then((team, x) => mock.addPlayers(new[] {new Player()}, team))
                .Then((data, x) => mock.Team(1))
                .Then((team, x) => Assert.AreEqual(1, team.Roster.Count));
        }
    }
}
