namespace WpfTestApp.Features.Documentation
{
    using Miruken.Mvc;

    public class DocumentationController: Controller
    {
        public string Description { get; set; }

        public void Index(string description)
        {
            Description = description;
            Show<Documentation>();
        }

        public void Done()
        {
            EndContext();
        }
    }
}
