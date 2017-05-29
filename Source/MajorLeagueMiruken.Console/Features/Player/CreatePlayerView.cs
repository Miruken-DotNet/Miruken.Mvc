namespace MajorLeagueMiruken.Console.Features.Team
{
    using System;
    using System.Linq;
    using Miruken.Mvc.Console;
    using Miruken.Mvc.Console.Forms;
    using Player;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class CreatePlayerView : View<CreatePlayerController>
    {
        private readonly Buffer _buffer;

        private Form _form;

        public CreatePlayerView()
        {
            _buffer  = new Buffer();
            Content = _buffer;
        }

        public override void Initialize()
        {
            _buffer.WriteLine("Create Player");
            _buffer.WriteLine();
            //_buffer.WriteLine(_menu.ToString());
            _buffer.Seperator();
            _buffer.WriteLine();

            _form = new Form(
                new Question("First Name?", x => Controller.Player.FirstName = x)
                    .Required("First Name is required"),

                new Question("Last Name?", x => Controller.Player.LastName = x)
                    .Required("Last Name is required"),

                new Question( "Birthdate?", x => Controller.Player.Birthdate = DateTime.Parse(x))
                    .Date()
                    .Required("Age is required"),

                new Question("Number?", x => Controller.Player.Number = int.Parse(x))
                    .Number()
                    .Required("Number is required"),

                new InputList(
                    "Team?",
                    Controller.Teams.OrderBy(x => x.Name).Select(x => x.Name).ToArray(),
                    x =>
                    {
                        Controller.Player.Team =
                            Controller.Teams.FirstOrDefault(t => t.Name == x );
                    }));
        }

        public void CompleteForm()
        {
            _form.Handle(_buffer, InputLocation.Inline);
            Controller.CreatePlayer();
        }
    }
}
