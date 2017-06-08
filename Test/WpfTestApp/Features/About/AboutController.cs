namespace WpfTestApp.Features.About
{
    using Miruken.Mvc;

    public class AboutController: Controller
    {
        public void About()
        {
            Show<About>().Disposed += EndContext;
        }

        public void Back()
        {
            EndContext();
        }
    }
}
