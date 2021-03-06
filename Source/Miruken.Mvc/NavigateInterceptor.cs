﻿#if NETFULL
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

            object result  = null;
            var methodCall = (IMethodCallMessage)msg;
            var method     = (MethodInfo)methodCall.MethodBase;
            var args       = methodCall.Args;

            void Dispatch(C controller)
            {
                var m = method;
                if (_controller == null)
                {
                    _controller = controller;
                }
                else if (controller != _controller)
                {
                    m = RuntimeHelper.SelectMethod(method, controller.GetType());
                }

                result = m.Invoke(controller, BindingFlags.Instance
                     | BindingFlags.Public, null, args, CultureInfo.InvariantCulture);
            }

            try
            {
                if (_controller == null)
                    _handler.Navigate(_style, (Action<C>) Dispatch);
                else
                    Dispatch(_controller);

                return new ReturnMessage(
                    result, args, methodCall.ArgCount,
                    methodCall.LogicalCallContext, methodCall);
            }
            catch (TargetInvocationException tex)
            {
                return new ReturnMessage(tex.InnerException, methodCall);
            }
        }

        private static void EnsureValidAction(IMessage msg)
        {
            if (!(msg is IMethodCallMessage methodCall))
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
#endif
