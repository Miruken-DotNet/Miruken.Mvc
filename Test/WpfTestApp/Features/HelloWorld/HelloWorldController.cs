namespace WpfTestApp.Features.HelloWorld
{
    using About;
    using Documentation;
    using Error;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;
    using Miruken.Mvc.Options;
    using Miruken.Mvc.Wpf.Animation;
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
            Push<DocumentationController>(IO
                //.Zoom()
                //.Fade()
                .RollInOut(Origin.MiddleCenter)
                .Behavior(new TimelineBehavior { DecelerationRatio = .9 }))
                .Index();
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
            Push<SettingsController>(IO
                .Zoom(scale:.8)
                .Fade()
                .WipeConvergeIn())
                .Configure();
        }

        public void Delay()
        {
            Next<DocumentationController>().Delay(10000);
        }
    }
}
