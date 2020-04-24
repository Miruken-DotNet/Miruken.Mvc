namespace Miruken.Mvc.Wpf.TestApp.Features.Error
{
    using System;
    using Mvc;

    public class CtorErrorController : Controller
    {
        public CtorErrorController()
        {
            throw new Exception("Something bad happened");
        }

        public void DoNothing()
        {           
        }
    }
}
