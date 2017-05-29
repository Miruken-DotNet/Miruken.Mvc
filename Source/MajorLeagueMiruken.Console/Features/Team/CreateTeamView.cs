namespace MajorLeagueMiruken.Console.Features.Team
{
    using System;
    using Api;
    using Miruken.Concurrency;
    using Miruken.Mvc.Console;
    using Miruken.Mvc.Console.Forms;
    using Buffer = Miruken.Mvc.Console.Buffer;

    public class CreateTeamView : View<CreateTeamController>
    {
        private readonly Buffer _buffer;
        private readonly Form   _form;

        public CreateTeamView()
        {
            _buffer  = new Buffer();
            Content = _buffer;
            _form = new Form(
                new Question("Team Name?", x => Controller.Team.Name = x)
                    .Required("Team Name is required"),

                new InputList(
                    "Team Color?",
                    Enum.GetNames(typeof(Color)),
                    x =>
                    {
                        Color color;
                        if(Enum.TryParse(x, out color))
                            Controller.Team.Color = color;
                    }),

                new Question("Manager First Name?", x => Controller.Team.Manager.FirstName = x)
                    .Required("Manager First Name is required"),

                new Question("Manager Last Name?", x => Controller.Team.Manager.LastName = x)
                    .Required("Manager Last Name is required"),

                new Question("Coach First Name?", x => Controller.Team.Coach.FirstName = x)
                    .Required("Coach First Name is required"),

                new Question("Coach Last Name?",  x => Controller.Team.Coach.LastName = x)
                    .Required("Coach Last Name is required"));
        }

        public override void Initialize()
        {
            _buffer.WriteLine("Create Team");
            _buffer.WriteLine();
            _buffer.Seperator();
            _buffer.WriteLine();
        }

        public Promise CompleteForm()
        {
            _form.Handle(_buffer, InputLocation.Inline);
            return Controller.CreateTeam();
        }
    }
}
