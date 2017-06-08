namespace ConsoleTestApp.Features.A
{
    using System;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class AView : View<AController>
    {
        private readonly Menu menu;

        public AView()
        {
            var output = new Buffer();
            Content = output;
            output.Header("A View");
            output.WriteLine("A Feature");
            output.Block("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            menu = new Menu(
                new MenuItem("Quit",    ConsoleKey.Q, () => Controller.Quit()),
                new MenuItem("Forward", ConsoleKey.F, () => Controller.GoToBView()),
                new MenuItem("Hello",   ConsoleKey.H, () => Controller.GoToHello()),
                new MenuItem("Editing", ConsoleKey.T, () => Controller.GoToEditing()));
            output.WriteLine(menu.ToString());
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            menu.Listen(keyInfo);
        }
    };
}