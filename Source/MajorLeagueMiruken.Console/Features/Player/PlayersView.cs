namespace MajorLeagueMiruken.Console.Features.Player
{
    using System;
    using Api;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class PlayersView : View<PlayersController>
    {
        private readonly Buffer _buffer;
        private readonly Menu   _menu;
        private SelectList<Player> _selectList;

        public PlayersView()
        {
            _buffer = new Buffer();
            Content = _buffer;
            _menu = new Menu(
                new MenuItem("Create Player", ConsoleKey.C,
                () => Controller.GoToCreatePlayer()));
        }

        public override void Initialize()
        {
            base.Initialize();
            _selectList = new SelectList<Player>(
                Controller.Players,
                player => {Controller.GoToPlayer(player);});

            _buffer.WriteLine("Players");
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
