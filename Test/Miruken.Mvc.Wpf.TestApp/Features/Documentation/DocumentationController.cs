namespace Miruken.Mvc.Wpf.TestApp.Features.Documentation
{
    using HelloWorld;
    using Mvc;
    using Mvc.Animation;
    using Settings;

    public class DocumentationController: Controller
    {
        public void Index(string description)
        {
            Show<Documentation>(/*IO.RollOut(),*/ view =>
            {
                AddRegion(view.PartialRegion).Partial<HelloWorldController>(ctrl => ctrl.Greet());
                AddRegion(view.NextRegion).Next<HelloWorldController>(ctrl => ctrl.Greet());
                AddRegion(view.PushRegion).Push<SettingsController>(ctrl => ctrl.Configure("Hello"));
            });
        }

        public void Done()
        {
            Next<HelloWorldController>(IO
               .Fade()
               .Zoom()
               .RollOut(Origin.BottomLeft), ctrl => ctrl.Greet());

            //EndContext();
        }
    }
}
