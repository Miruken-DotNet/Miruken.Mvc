namespace WpfTestApp.Features.Documentation
{
    using HelloWorld;
    using Miruken.Mvc;
    using Miruken.Mvc.Animation;

    public class DocumentationController: Controller
    {
        public string Description { get; set; }

        public void Index(string description)
        {
            Description = description;
            Show<Documentation>(/*IO.RollOut()*/);
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
