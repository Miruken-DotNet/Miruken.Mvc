namespace Miruken.Mvc.Console
{
    public class Output
    {
        public OutputType OutputType { get; set; }
        public string     Text       { get; set; }

        public Output(string text, OutputType outputType)
        {
            Text       = text;
            OutputType = outputType;
        }
    }
}