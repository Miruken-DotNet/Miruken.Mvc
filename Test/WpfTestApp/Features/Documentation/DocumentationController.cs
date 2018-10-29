namespace WpfTestApp.Features.Documentation
{
    using HelloWorld;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;
    using Settings;

    public class DocumentationController: Controller
    {
        public void Index(string description)
        {
            Show<Documentation>(/*IO.RollOut(),*/ view =>
            {
                AddRegion(view.PartialRegion).Partial<HelloWorldController>().Greet();
                AddRegion(view.NextRegion).Next<HelloWorldController>().Greet();
                AddRegion(view.PushRegion).Push<SettingsController>().Configure("Hello");
            });
        }

        public void Done()
        {
            Next<HelloWorldController>(IO
               .Fade()
               .Zoom()
               .RollOut(Origin.BottomLeft))
               .Greet();

            //EndContext();
        }
    }
}
