namespace ConsoleTestApp.Features.B
{
    using C;
    using Miruken.Concurrency;

    public class BController : FeatureController
    {
        public Promise ShowBView()
        {
            Show<BView>();
            return Promise.Empty;
        }

        public Promise GoToCView()
        {
            Push<CController>().ShowCView();
            return Promise.Empty;
        }

    }
}