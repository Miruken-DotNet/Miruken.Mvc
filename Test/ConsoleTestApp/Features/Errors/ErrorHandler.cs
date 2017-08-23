namespace ConsoleTestApp.Features.Errors
{
    using System;
    using Miruken.Concurrency;

    public class ErrorHandler : IError
    {
        public Promise HandleException(
            Exception exception, object callback, object context)
        {
            return Promise.Rejected(new NotImplementedException());
        }
    }
}
