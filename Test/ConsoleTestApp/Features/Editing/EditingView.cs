namespace ConsoleTestApp.Features.Editing
{
    using System;
    using Miruken.Concurrency;
    using Miruken.Mvc.Console;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class EditingView : View<EditingController>
    {
        private readonly Buffer buffer;
        private readonly Menu   menu;
        private readonly Form   form;

        public EditingView()
        {
            buffer = new Buffer
            {
                Border  = new Thickness(1),
                Padding = new Thickness(2, 1)
            };
            Content = buffer;

            menu = new Menu(
                new MenuItem("Edit", ConsoleKey.E, () =>
                {
                    Controller.Phrase = buffer.EditWithDefaultProgram(Controller.Phrase);
                    buffer.WriteLine(Controller.Phrase);
                    Window.Update();
                    return Promise.Empty;
                }),
                new MenuItem("Edit With Vim", ConsoleKey.V, () =>
                {
                    Controller.Phrase = buffer.EditWithVim(Controller.Phrase);
                    buffer.WriteLine(Controller.Phrase);
                    Window.Update();
                    return Promise.Empty;
                }),
                new MenuItem("Edit In Place", ConsoleKey.P, () =>
                {
                    Controller.Phrase = buffer.Edit("Edit this? ", Controller.Phrase);
                    buffer.WriteLine(Controller.Phrase);
                    Window.Update();
                    return Promise.Empty;
                }),
                new MenuItem("Back", ConsoleKey.B, () => Controller.Back()));

            buffer.Header("Editing");
            buffer.WriteLine(menu.ToString());
            buffer.WriteLine();
        }

        public void AskQuestions()
        {
            Window.Update();
        }

        public override void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            base.KeyPressed(keyInfo);
            menu.Listen(keyInfo);
        }
    }
}