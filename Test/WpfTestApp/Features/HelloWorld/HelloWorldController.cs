namespace WpfTestApp.Features.HelloWorld
{
    using About;
    using Documentation;
    using Error;
    using Miruken.Infrastructure;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;
    using Miruken.Mvc.Options;
    using Miruken.Mvc.Views;
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
            Next<DocumentationController>(IO
                .Accelerate(.9)
                .Bounce(2.5, 2)
                .ZoomIn(Origin.MiddleRight, 10.0, 2.Sec()))
                .Index("Miruken is an application framework that embraces composition, convention, decoration, intention, and aspect-oriented programming. Miru and ken are Japanese words that both mean 'view'. Put them together and you have Miruken. It means 'a view of a view' and alludes to composition.");
            /*
            Push<DocumentationController>(IO
                .Fade()
                .Accelerate(.9)
                .FlipInOut(duration:1.5.Sec()))
                .Index();
            */
        }

        public void About()
        {
            Push<AboutController>(IO.Modal()).About();
        }

        public void AboutBack()
        {
            Next<AboutController>(IO.Push()).AboutBack();
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
            Show(IO
                .Fade()
                .Accelerate(.9)
                .SpinInOut(1.5.Sec())
                .Region<SettingsController>(
                    ctrl => ctrl.Configure("Notifications") ));

            /*
            Push<SettingsController>(IO
                .Fade()
                .Accelerate(.9)
                .SpinInOut(1.5.Sec()))
                .Configure("Notifications");
                */
        }
    }
}
