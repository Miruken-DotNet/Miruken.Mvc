using Miruken.Context;
using Miruken.Mvc.Policy;

namespace Miruken.Mvc
{
    public interface IController : IContextual, IPolicyOwner<ControllerPolicy>
    {
    }
}
