namespace WpfTestApp.Features.Error
{
    using System;
    using Miruken.Mvc;

    public class FaultyController : Controller
    {
        public void FailBeforeView()
        {
            throw new InvalidOperationException("Failed before view");
        }

        public void FailAfterView()
        {
            Show<Faulty>();
            throw new InvalidOperationException("Failed aftdr view");
        }
    }
}
