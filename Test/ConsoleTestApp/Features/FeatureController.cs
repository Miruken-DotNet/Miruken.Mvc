namespace ConsoleTestApp.Features
{
    using Miruken.Concurrency;
    using Miruken.Mvc;

    public class FeatureController : Controller
    {
        public Promise Back()
        {
            EndContext();
            return Promise.Empty;
        }
    }
}
