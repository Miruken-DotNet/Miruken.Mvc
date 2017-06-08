namespace ConsoleTestApp.Features.C
{
    using Miruken.Concurrency;

    public class CController : FeatureController
    {
        public Promise ShowCView()
        {
            Show<CView>();
            return Promise.Empty;
        }
    }
}