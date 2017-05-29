namespace MajorLeagueMiruken.Console.Features.Team
{
    using System;
    using Api;
    using Miruken.Mvc.Console;
    using Miruken.Mvc.Console.Forms;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class EditTeamView : View<EditTeamController>
    {
        private readonly Buffer _buffer;

        private Form _form;

        public EditTeamView()
        {
            _buffer  = new Buffer();
            Content = _buffer;
        }

        public override void Initialize()
        {
            _buffer.WriteLine("Edit Team");
            _buffer.WriteLine();
            _buffer.Seperator();
            _buffer.WriteLine();

            _form = new Form(
                new Question(
                    "Team Name:",
                    Controller.Team.Name,
                    x => Controller.Team.Name = x)
                        .Required("Team Name is required"),

                new InputList(
                    "Team Color?",
                    Controller.Team.Color.ToString(),
                    Enum.GetNames(typeof(Color)),
                    x =>
                    {
                        Color color;
                        if(Enum.TryParse(x, out color))
                            Controller.Team.Color = color;
                    }),

                new Question(
                    "Manager First Name?",
                    Controller.Team.Manager.FirstName,
                    x => Controller.Team.Manager.FirstName = x)
                        .Required("Manager First Name is required"),

                new Question(
                    "Manager Last Name?",
                    Controller.Team.Manager.LastName,
                    x => Controller.Team.Manager.LastName = x)
                        .Required("Manager Last Name is required"),

                new Question(
                    "Coach First Name?",
                    Controller.Team.Coach.FirstName,
                    x => Controller.Team.Coach.FirstName = x)
                        .Required("Coach First Name is required"),

                new Question(
                    "Coach Last Name?",
                    Controller.Team.Coach.LastName,
                    x => Controller.Team.Coach.LastName = x)
                        .Required("Coach Last Name is required"));
        }

        public void CompleteForm()
        {
            _form.Handle(_buffer, InputLocation.Inline);
            Controller.UpdateTeam();
        }
    }
}
