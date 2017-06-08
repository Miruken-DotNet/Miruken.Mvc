namespace ConsoleTestApp.Features.Errors
{
    using System;

    public class ErrorHandler : IError
    {
        public bool HandleException(Exception exception, object context = null)
        {
            throw new NotImplementedException();
        }
    }
}
