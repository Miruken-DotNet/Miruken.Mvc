namespace WpfTestApp.Features.Documentation
{
    using System.Threading.Tasks;
    using HelloWorld;
    using Miruken.Mvc;

    public class DocumentationController: Controller
    {
        public void Index()
        {
            Show<Documentation>();
        }

        public void Delay(int delay)
        {
            Task.Delay(delay)
                .ContinueWith(t => Show<Documentation>());
        }

        public void Done()
        {
            Next<HelloWorldController>().Greet();
        }
    }
}
