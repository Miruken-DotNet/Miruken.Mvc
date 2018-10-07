using Miruken.Context;
using Miruken.Mvc.Policy;

namespace Miruken.Mvc
{
    using Callback;

    public interface IController : IContextual, IPolicyOwner<ControllerPolicy>
    {
        IHandler IO { get; set; }
    }
}
