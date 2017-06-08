namespace WpfTestApp.Features.Documentation
{
    using HelloWorld;
    using Miruken.Mvc;

    public class DocumentationController: Controller
    {
        public void Index()
        {
            Show<Documentation>();
        }

        public void Done()
        {
            Next<HelloWorldController>().Greet();
        }
    }
}
