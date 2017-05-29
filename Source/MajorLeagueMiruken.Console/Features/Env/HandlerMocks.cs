namespace MajorLeagueMiruken.Console.Features.Env
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Miruken.Concurrency;
    using Player;
    using Team;

    public class HandlerMocks : ITeam, IPlayer
    {
        private int _id;

        private int _nextId => ++_id;

        private readonly List<Team> _teams = new List<Team>();

        private readonly List<Player> _players = new List<Player>();

        public HandlerMocks()
        {
            _teams.AddRange(new[]
            {
                new Team {
                    Id      = _nextId,
                    Name    = "Dallas",
                    Color   = Color.Blue,
                    Coach   = new Person { FirstName = "David", LastName = "O'Hara" },
                    Manager = new Person { FirstName = "Ric",   LastName = "DeAnda" }},
                new Team {
                    Id      = _nextId,
                    Name    = "College Station",
                    Color   = Color.Maroon,
                    Coach   = new Person { FirstName = "Ed",   LastName = "Grannan" },
                    Manager = new Person { FirstName = "Mike", LastName = "Abney" }},
                new Team {
                    Id      = _nextId,
                    Name    = "Houston",
                    Color   = Color.LightBlue,
                    Coach   = new Person { FirstName = "Ken",    LastName = "Howard" },
                    Manager = new Person { FirstName = "Devlin", LastName = "Liles" }},
                new Team {
                    Id      = _nextId,
                    Name    = "Columbus",
                    Color   = Color.Red,
                    Coach   = new Person { FirstName = "Mark",    LastName = "Kovacevich" },
                    Manager = new Person { FirstName = "Jacquie", LastName = "Bickel" }},
                new Team {
                    Id      = _nextId,
                    Name    = "Calgary",
                    Color   = Color.White,
                    Coach   = new Person { FirstName = "Wendy", LastName = "Brown" },
                    Manager = new Person { FirstName = "Brian", LastName = "Donaldson" }},
                new Team {
                    Id      = _nextId,
                    Name    = "Minneapolis",
                    Color   = Color.Yellow,
                    Coach   = new Person { FirstName = "Barb",  LastName = "Gurstelle" },
                    Manager = new Person { FirstName ="Leroy" , LastName = "Thydean" }}
            });

            _players.AddRange(new []
            {
                new Player{ Id = _nextId, TeamId = 1, FirstName = "Cori",    LastName = "Drew",      Birthdate = DateTime.Parse("06/06/1980"), Number = 1  },
                new Player{ Id = _nextId, TeamId = 1, FirstName = "Craig",   LastName = "Neuwirt",   Birthdate = DateTime.Parse("07/19/1970"), Number = 22 },
                new Player{ Id = _nextId, TeamId = 1, FirstName = "Michael", LastName = "Dudley",    Birthdate = DateTime.Parse("08/28/1977"), Number = 7  },
                new Player{ Id = _nextId, TeamId = 1, FirstName = "Kevin",   LastName = "Baker",     Birthdate = DateTime.Parse("02/02/1980"), Number = 3  },
                new Player{ Id = _nextId,             FirstName = "Tim",     LastName = "Rayburn",   Birthdate = DateTime.Parse("01/02/1976"), Number = 55 },
                new Player{ Id = _nextId, TeamId = 5, FirstName = "Glen",    LastName = "Donaldson", Birthdate = DateTime.Parse("01/01/1976"), Number = 11 }
            });

        }

        #region ITeam

        public Promise<Team> CreateTeam(Team team)
        {
            team.Id = _nextId;
            _teams.Add(team);
            return Promise.Resolved(team);
        }

        public Promise DeleteTeam(Team team)
        {
            var item = _teams.FirstOrDefault(x => x.Id == team.Id);
            if (item != null)
                _teams.Remove(item);
            return Promise.Empty;
        }

        public Promise<Team[]> Teams()
        {
            return Promise.Resolved(_teams.ToArray());
        }

        public Promise<Team> Team(int id)
        {
            return Promise.Resolved(_teams.FirstOrDefault(x => x.Id == id));
        }

        public Promise<Team> UpdateTeam(Team data)
        {
            var team = _teams.FirstOrDefault(x => x.Id == data.Id);
            if(team != null)
            {
                team.Coach   = data.Coach;
                team.Color   = data.Color;
                team.Manager = data.Manager;
                team.Name    = data.Name;
                team.Roster  = data.Roster;
            };
            return Promise.Resolved(team);
        }

        public Promise addPlayers(Player[] players, Team team)
        {
            var targetTeam = _teams.FirstOrDefault(x => x.Id == team.Id);
            if(targetTeam == null )return Promise.Empty;

            foreach (var player in players)
            {
                if(targetTeam.Roster.All(x => x.Id != player.Id))
                    targetTeam.Roster.Add(player);
            }
            return Promise.Empty;
        }

        #endregion

        #region IPlayer

        public Promise<Player> CreatePlayer(Player player)
        {
            player.Id = _nextId;
            _players.Add(player);
            return Promise.Resolved(player);
        }

        public Promise DeletePlayer(Player player)
        {
            var item = _players.FirstOrDefault(x => x.Id == player.Id);
            if (item != null)
                _players.Remove(item);
            return Promise.Empty;
        }

        public Promise<Player> Player(int id)
        {
            return Promise.Resolved(_players.FirstOrDefault(x => x.Id == id));
        }

        public Promise<Player[]> Players()
        {
            return Promise.Resolved(_players.ToArray());
        }

        public Promise<Player> UpdatePlayer(Player data)
        {
            var player = _players.FirstOrDefault(x => x.Id == data.Id);
            if(player != null)
            {
                player.Number = data.Number;
                player.Birthdate = data.Birthdate;
                player.FirstName = data.FirstName;
                player.LastName = data.LastName;
            };
            return Promise.Resolved(player);
        }

        #endregion
    }
}
