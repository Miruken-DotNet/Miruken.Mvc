using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Miruken.Callback;
using static Miruken.Protocol;

namespace Miruken.Mvc
{
    public class NavigateInterceptor<C> : RealProxy, IRemotingTypeInfo
        where C : class, IController
    {
        private readonly IHandler _handler;
        private readonly NavigationStyle _style;
        private bool _completed;

        public NavigateInterceptor(
            IHandler handler, NavigationStyle style)
            : base(typeof(C))
        {
            _handler = handler;
            _style   = style;
        }

        public string TypeName { get; set; }

        public bool CanCastTo(Type fromType, object o)
        {
            return fromType.IsAssignableFrom(typeof(C));
        }

        public override IMessage Invoke(IMessage msg)
        {
            EnsureValidAction(msg);

            if (_completed)
                throw new InvalidOperationException(
                    $"Controller {typeof(C).FullName} has already completed navigation");

            var methodCall = (IMethodCallMessage)msg;

            Func<C, object> action = controller =>
            {
                _completed = true;
                return methodCall.MethodBase.Invoke(controller,
                    BindingFlags.Instance | BindingFlags.Public,
                    null, methodCall.Args, null);
            };

            try
            {
                return new ReturnMessage(
                    P<INavigate>(_handler).Navigate(action, _style),
                    methodCall.Args, methodCall.ArgCount,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch (TargetInvocationException tex)
            {
                return new ReturnMessage(tex.InnerException, methodCall);
            }
        }

        private static void EnsureValidAction(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            if (methodCall == null)
                throw new InvalidOperationException("Action must be done on a method");

            var method     = methodCall.MethodBase;
            var methodName = methodCall.MethodName;

            if (method.IsSpecialName && (methodName.StartsWith("get_") ||
                methodName.StartsWith("_set")))
                throw new InvalidOperationException(
                    $"Action must be done on a method:  {methodName} is not a method");
        }
    }
}
