namespace MajorLeagueMiruken.Console.Features.Error
{
    using System;
    using Miruken.Callback;
    using Miruken.Error;

    public interface IError : IErrors, IResolving
    {
    }

    public class ErrorHandler : IError
    {
        public bool HandleException(Exception exception, object context = null)
        {
            throw new NotImplementedException();
        }
    }
}
