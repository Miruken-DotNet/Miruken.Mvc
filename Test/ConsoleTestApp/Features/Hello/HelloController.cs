namespace ConsoleTestApp.Features.Hello
{
    using Miruken.Concurrency;

    public class HelloController : FeatureController
    {
        public string FirstName { get; set; }
        public string LastName  { get; set; }
        public string Fruit     { get; set; }
        public string Country   { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public Promise ShowHello()
        {
            HelloView view = null;
            Show<HelloView>(v => view = v);
            view.AskQuestions();
            return Promise.Empty;
        }
    }
}
