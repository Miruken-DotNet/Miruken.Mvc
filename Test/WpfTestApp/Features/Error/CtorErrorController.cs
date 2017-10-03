namespace WpfTestApp.Features.Error
{
    using System;
    using Miruken.Mvc;

    public class CtorErrorController : Controller
    {
        public CtorErrorController()
        {
            throw new Exception("Something bad happended");
        }

        public void DoNothing()
        {           
        }
    }
}
