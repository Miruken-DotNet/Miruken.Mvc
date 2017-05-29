namespace Miruken.Mvc.Console
{
    using System.Collections.Generic;
    using System.Linq;

    public class Form
    {
        public List<Input> Inputs { get; } = new List<Input>();

        public Form(params Input[] inputs)
        {
            foreach (var input in inputs)
                Inputs.Add(input);
        }

        public void Handle(Buffer buffer, InputLocation inputLocation)
        {
            var longestText = Inputs.Max(x => x.Text.Length);
            foreach (var input in Inputs)
            {
                input.Handle(buffer, longestText, inputLocation);
            }
        }
    }
}