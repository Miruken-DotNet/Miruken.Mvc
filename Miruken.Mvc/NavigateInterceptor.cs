﻿using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Miruken.Callback;
using Miruken.MVC;
using static Miruken.Protocol;

namespace Miruken.Mvc
{
    public class NavigateInterceptor<C> : RealProxy, IRemotingTypeInfo
        where C : IController
    {
        private readonly IHandler _handler;
        private readonly NavigationStyle _style;

        public NavigateInterceptor(
            IHandler handler, NavigationStyle style)
            : base(typeof(C))
        {
            _handler = handler;
            _style = style;
        }

        public string TypeName { get; set; }

        public bool CanCastTo(Type fromType, object o)
        {
            return fromType.IsAssignableFrom(typeof(C));
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = (IMethodCallMessage)msg;

            Func<C, object> action = controller => 
                methodCall.MethodBase.Invoke(controller,
                    BindingFlags.Instance | BindingFlags.Public, 
                    null, methodCall.Args, null);

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
    }
}