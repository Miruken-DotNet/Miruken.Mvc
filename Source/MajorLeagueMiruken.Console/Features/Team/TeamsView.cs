namespace MajorLeagueMiruken.Console.Features.Team
{
    using System;
    using Api;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class TeamsView : View<TeamsController>
    {
        private readonly Buffer _buffer;
        private readonly Menu   _menu;
        private SelectList<Team> _selectList;

        public TeamsView()
        {
            _buffer  = new Buffer();
            Content = _buffer;
            _menu = new Menu(
                new MenuItem("Create Team", ConsoleKey.C,
                () => Controller.GoToCreateTeam()));
        }

        public override void Initialize()
        {
            _selectList = new SelectList<Team>(
                Controller.Teams,
                team => {Controller.GoToTeam(team);});

            _buffer.WriteLine("Teams");
            _buffer.WriteLine();
            _buffer.WriteLine(_menu.ToString());
            _buffer.Seperator();
            _buffer.WriteLine();
            _buffer.WriteLine(_selectList.ToString());
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            _menu.Listen(keyInfo);
            _selectList.Listen(keyInfo);
        }
    }
}
