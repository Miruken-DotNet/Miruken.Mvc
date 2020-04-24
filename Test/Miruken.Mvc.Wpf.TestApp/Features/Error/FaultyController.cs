namespace Miruken.Mvc.Wpf.TestApp.Features.Error
{
    using System;
    using Mvc;

    public class FaultyController : Controller
    {
        public void FailBeforeView()
        {
            throw new InvalidOperationException("Failed before view");
        }

        public void FailAfterView()
        {
            Show<Faulty>();
            throw new InvalidOperationException("Failed after view");
        }
    }
}
