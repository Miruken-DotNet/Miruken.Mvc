namespace Miruken.Mvc
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using Callback;
    using Infrastructure;

    public class NavigationRequest
    {
        public NavigationRequest(Type controllerType, MethodInfo action,
                                 object[] args, NavigationStyle style)
        {
            ControllerType = controllerType;
            Action         = action;
            Args           = args;
            Style          = style;
        }
        public Type            ControllerType { get; }
        public MethodInfo      Action         { get; }
        public object[]        Args           { get; }
        public NavigationStyle Style          { get; }
    }

    public class NavigateInterceptor<C> : RealProxy, IRemotingTypeInfo
        where C : class, IController
    {
        private readonly IHandler _handler;
        private readonly NavigationStyle _style;
        private C _controller;

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
            if (_controller == null)
                EnsureValidAction(msg);

            var methodCall = (IMethodCallMessage)msg;
            var method     = (MethodInfo)methodCall.MethodBase;
            var args       = methodCall.Args;

            Func<C, object> action = controller =>
            {
                var m = method;
                if (_controller == null)
                    _controller = controller;
                else if (controller != _controller)
                    m = RuntimeHelper.SelectMethod(method, controller.GetType());
                return m.Invoke(controller,
                    BindingFlags.Instance | BindingFlags.Public,
                    null, args, CultureInfo.InvariantCulture);
            };

            try
            {
                object result;
                if (_controller == null)
                {
                    var request = new NavigationRequest(typeof(C), method, args, _style);
                    result = _handler.Provide(request).Proxy<INavigate>()
                        .Navigate(action, _style);
                }
                else
                    result = action(_controller);

                return new ReturnMessage(result, args, methodCall.ArgCount,
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
                throw new InvalidOperationException(
                    "Initial action must be done on a method");

            var method     = methodCall.MethodBase;
            var methodName = methodCall.MethodName;

            if (method.IsSpecialName && (methodName.StartsWith("get_") ||
                methodName.StartsWith("_set")))
                throw new InvalidOperationException(
                    $"Initial action must be done on a method:  {methodName} is not a method");
        }
    }
}
