namespace Miruken.Mvc
{
    using Miruken.Context;
    using Miruken.Mvc.Policy;

    public interface IController : IContextual, IPolicyOwner<ControllerPolicy>
    {
    }
}
