using SixFlags.CF.Miruken.Context;
using SixFlags.CF.Miruken.MVC.Policy;

namespace SixFlags.CF.Miruken.MVC
{
    public interface IController : IContextual, IPolicyOwner<ControllerPolicy>
    {
    }
}
