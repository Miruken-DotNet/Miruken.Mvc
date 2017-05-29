namespace MajorLeagueMiruken.Api
{
    using System.Text;

    public class Player : Person
    {
        public int  Number { get; set; }
        public int  TeamId { get; set; }
        public Team Team   { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{FullName} #{Number}");
            builder.AppendLine($"  Team    : {Team?.Name}");
            builder.AppendLine($"  Birthday: {Birthdate:MMMM dd yyyy}");
            builder.AppendLine($"  Age     : {Age}");
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
