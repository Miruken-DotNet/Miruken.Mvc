namespace Miruken.Mvc
{
    using System;
    using Context;

    public class NavigationException : Exception
    {
        public NavigationException(Context context)
        {
            Context = context;
        }

        public NavigationException(Context context, string message)
            : base(message)
        {
            Context = context;
        }

        public NavigationException(Context context, string message, Exception innException)
            : base(message, innException)
        {
            Context = context;
        }

        public Context Context { get; }
    }
}
