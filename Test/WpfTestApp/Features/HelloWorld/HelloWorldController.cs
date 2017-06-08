namespace WpfTestApp.Features.HelloWorld
{
    using About;
    using Documentation;
    using Miruken.Mvc;
    using Miruken.Mvc.Options;

    public class HelloWorldController : Controller
    {
        public void Greet()
        {
            Show<HelloWorld>();
        }

        public void Documentation()
        {
            Next<DocumentationController>().Index();
        }

        public void About()
        {
            Push<AboutController>(IO.Modal()).About();
        }
    }
}
