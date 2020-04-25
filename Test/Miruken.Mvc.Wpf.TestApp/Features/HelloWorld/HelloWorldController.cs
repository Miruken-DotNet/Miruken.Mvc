namespace Miruken.Mvc.Wpf.TestApp.Features.HelloWorld
{
    using About;
    using Animation;
    using Documentation;
    using Error;
    using Infrastructure;
    using Mvc;
    using Mvc.Animation;
    using Options;
    using Settings;

    public class HelloWorldController : Controller
    {
        public void Greet()
        {
            Description = "Hello World!!";
            Show<HelloWorld>();
        }

        public string Description { get; set; }

        public bool CanDocumentation()
        {
            return true;
        }

        public void Documentation()
        {
            Next<DocumentationController>(IO
                .Accelerate(.9)
                .Bounce(2.5, 2)
                .ZoomIn(Origin.MiddleRight, 10.0, 2.Sec()),
                    ctrl => ctrl.Index("Miruken is an application framework that embraces composition, convention, decoration, intention, and aspect-oriented programming. Miru and ken are Japanese words that both mean 'view'. Put them together and you have Miruken. It means 'a view of a view' and alludes to composition."));
            /*
            Push<DocumentationController>(IO
                .Fade()
                .Accelerate(.9)
                .FlipInOut(duration:1.5.Sec()), ctrl => ctrl.Index());
            */
        }

        public bool CanAbout()
        {
            return true;
        }

        public void About()
        {
            Push<AboutController>(IO.Modal(), ctrl => ctrl.About());
        }

        public void AboutBack()
        {
            Next<AboutController>(IO.Push(), ctrl => ctrl.AboutBack());
        }

        public void CtorError()
        {
            Push<CtorErrorController>(ctrl => ctrl.DoNothing());
        }

        public void ErrorBeforeView()
        {
            Next<FaultyController>(ctrl => ctrl.FailBeforeView());
        }

        public void ErrorAfterView()
        {
            Next<FaultyController>(ctrl => ctrl.FailAfterView());
        }

        public void PushErrorBeforeView()
        {
            Push<FaultyController>(ctrl => ctrl.FailBeforeView());
        }

        public void PushErrorAfterView()
        {
            Push<FaultyController>(ctrl => ctrl.FailAfterView());
        }

        public void Settings()
        {
            /*
            Show(IO
                .Fade()
                .Accelerate(.9)
                .SpinInOut(1.5.Sec())
                .Region<SettingsController>(
                    ctrl => ctrl.Configure("Notifications") ));
            */

            Push<SettingsController>(IO
                .Fade()
                .Accelerate(.9)
                .SpinInOut(1.5.Sec()),
                    ctrl => ctrl.Configure("Notifications"));
        }
    }
}
