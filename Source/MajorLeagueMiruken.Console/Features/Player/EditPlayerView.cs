namespace MajorLeagueMiruken.Console.Features.Player
{
    using System;
    using System.Linq;
    using Api;
    using Miruken.Mvc.Console;
    using Miruken.Mvc.Console.Forms;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class EditPlayerView : View<EditPlayerController>
    {
        private readonly Buffer _buffer;

        private Form _form;

        public EditPlayerView()
        {
            _buffer  = new Buffer();
            Content = _buffer;
        }

        public override void Initialize()
        {
            _buffer.WriteLine("Edit Player");
            _buffer.WriteLine();
            //_buffer.WriteLine(_menu.ToString());
            _buffer.Seperator();
            _buffer.WriteLine();

            _form = new Form(
                new Question(
                    "First Name?",
                    Controller.Player.FirstName,
                    x => Controller.Player.FirstName = x)
                    .Required("First Name is required"),

                new Question(
                    "Last Name?",
                    Controller.Player.LastName,
                    x => Controller.Player.LastName = x)
                    .Required("Last Name is required"),

                new Question(
                    "Birthdate?",
                    Controller?.Player?.Birthdate.ToString("MMMM dd yyyy"),
                    x => Controller.Player.Birthdate = DateTime.Parse(x))
                    .Date()
                    .Required("Age is required"),

                new Question(
                    "Number?",
                    Controller.Player.Number.ToString(),
                    x => Controller.Player.Number = int.Parse(x))
                    .Number()
                    .Required("Number is required"),

                new InputList(
                    "Team?",
                    Controller?.Player?.Team?.Name,
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
            Controller.UpdatePlayer();
        }
    }
}
