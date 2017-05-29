namespace ConsoleTestApp.Features.A
{
    using System.Threading.Tasks;
    using B;
    using Editing;
    using Hello;
    using Miruken.Concurrency;
    using Miruken.Mvc;
    using Miruken.Mvc.Console;

    public class AController : Controller
    {
        public void ShowAView()
        {
            Show<AView>();
        }

        public Promise GoToBView()
        {
            return Push<BController>().ShowBView();
        }

        public Promise GoToHello()
        {
            return Push<HelloController>().ShowHello();
        }

        public Promise GoToEditing()
        {
            return Push<EditingController>().ShowEditing();
        }

        public Task Quit()
        {
            Window.Quit = true;
            return Promise.Empty;
        }
    }
}
