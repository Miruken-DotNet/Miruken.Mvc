namespace MajorLeagueMiruken.Api
{
    using System.Collections.Generic;
    using System.Text;
    using FluentValidation;

    public class Team
    {
        public int      Id         { get; set; }
        public string   Name       { get; set; }
        public Color    Color      { get; set; }
        public Person   Manager    { get; set; }
        public Person   Coach      { get; set; }
        public List<Player> Roster { get; set; } = new List<Player>();

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{Name}");
            builder.AppendLine($"  Color  : {Color}");
            builder.AppendLine($"  Manager: {Manager?.FullName}");
            builder.AppendLine($"  Coach  : {Coach?.FullName}");
            builder.AppendLine();
            return builder.ToString();

        }
    }

    public class TeamIntegrity : AbstractValidator<Team>
    {
        public TeamIntegrity()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Color).NotEmpty();
            RuleFor(x => x.Coach).NotNull();
            RuleFor(x => x.Coach).SetValidator(new CoachManagerIntegrity());
            RuleFor(x => x.Manager).SetValidator(new CoachManagerIntegrity());
        }
    }

    public class CoachManagerIntegrity : AbstractValidator<Person>
    {
        public CoachManagerIntegrity()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}