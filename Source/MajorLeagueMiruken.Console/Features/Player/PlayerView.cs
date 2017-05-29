namespace MajorLeagueMiruken.Console.Features.Player
{
    using System;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class PlayerView : View<PlayerController>
    {
        private readonly Buffer _buffer;
        private readonly Menu   _menu;

        public PlayerView()
        {
            _buffer = new Buffer();
            Content = _buffer;
            _menu = new Menu(
                new MenuItem("Edit Player", ConsoleKey.E,
                    () => Controller.GoToEditPlayer()),
                new MenuItem("Create Player", ConsoleKey.C,
                    () => Controller.GoToCreatePlayer()));
        }

        public override void Initialize()
        {
            base.Initialize();
            _buffer.WriteLine("Player");
            _buffer.WriteLine();
            _buffer.WriteLine(_menu.ToString());
            _buffer.Seperator();
            _buffer.WriteLine();
            _buffer.WriteLine(Controller.Player?.ToString());
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            _menu.Listen(keyInfo);
        }
    }
}
