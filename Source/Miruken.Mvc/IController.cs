using Miruken.Context;

namespace Miruken.Mvc
{
    using Callback;

    public interface IController : IContextual
    {
        IHandler IO { set; }
    }
}
