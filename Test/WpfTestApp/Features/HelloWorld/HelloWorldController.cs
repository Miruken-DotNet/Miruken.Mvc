namespace WpfTestApp.Features.HelloWorld
{
    using System.Threading.Tasks;
    using About;
    using Documentation;
    using Error;
    using Miruken.Mvc;
    using Miruken.Mvc.Options;
    using Settings;

    public class HelloWorldController : Controller
    {
        public void Greet()
        {
            Show<HelloWorld>();
        }

        public bool CanDocumentation()
        {
            return true;
        }

        public void Documentation()
        {
            Next<DocumentationController>().Index();
        }

        public void About()
        {
            Push<AboutController>(IO.Modal()).About();
        }

        public void CtorError()
        {
            Push<CtorErrorController>().DoNothing();
        }

        public void ErrorBeforeView()
        {
            Next<FaultyController>().FailBeforeView();
        }

        public void ErrorAfterView()
        {
            Next<FaultyController>().FailAfterView();
        }

        public void PushErrorBeforeView()
        {
            Push<FaultyController>().FailBeforeView();
        }

        public void PushErrorAfterView()
        {
            Push<FaultyController>().FailAfterView();
        }

        public void Settings()
        {
            Next<SettingsController>().Configure();
        }

        public void Delay()
        {
            Next<DocumentationController>().Delay(10000);
        }
    }
}
