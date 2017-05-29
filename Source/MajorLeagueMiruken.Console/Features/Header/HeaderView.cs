namespace MajorLeagueMiruken.Console.Features.Header
{
    using System;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class HeaderView : View<HeaderController>
    {
        private readonly Buffer _buffer;
        private readonly Menu   _menu;

        public HeaderView()
        {
            _buffer = new Buffer();
            Content = _buffer;
            _buffer.WriteLine("Major Leage Miruken");
            _buffer.WriteLine();
            _menu = new Menu(
                new MenuItem("Teams",   ConsoleKey.T, ()=> Controller.GoToTeams()),
                new MenuItem("Players", ConsoleKey.P, ()=> Controller.GoToPlayers()));
            _buffer.Write(_menu.ToString());

        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            _menu.Listen(keyInfo);
        }
    }
}