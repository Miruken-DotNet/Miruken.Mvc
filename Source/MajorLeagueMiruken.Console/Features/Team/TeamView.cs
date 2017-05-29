namespace MajorLeagueMiruken.Console.Features.Team
{
    using System;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class TeamView : View<TeamController>
    {
        private readonly Buffer _buffer;
        private readonly Menu   _menu;

        public TeamView()
        {
            _buffer = new Buffer();
            Content = _buffer;
            _menu = new Menu(
                new MenuItem("Edit Team", ConsoleKey.E,
                    () => Controller.GoToEditTeam()),
                new MenuItem("Create Team", ConsoleKey.C,
                    () => Controller.GoToCreateTeam()));
        }

        public override void Initialize()
        {
            base.Initialize();
            _buffer.WriteLine("Team");
            _buffer.WriteLine();
            _buffer.WriteLine(_menu.ToString());
            _buffer.Seperator();
            _buffer.WriteLine();
            _buffer.WriteLine(Controller.Team?.ToString());
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            _menu.Listen(keyInfo);
        }
    }
}
