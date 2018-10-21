using Miruken.Callback;
using Miruken.Concurrency;
using Miruken.Mvc.Policy;
using Miruken.Mvc.Views;

namespace Miruken.Mvc
{
    using Context = Context.Context;

    public static class ControllerExtensions
    {
        public static IController Track(this IController controller)
        {
            controller?.Policy?.Track();
            return controller;
        }

        public static IController Retain(this IController controller)
        {
            controller?.Policy?.Retain();
            return controller;
        }

        public static IController Release(this IController controller)
        {
            controller?.Policy?.Release();
            return controller;
        }

        public static IController DependsOn(this IController controller, IView dependency)
        {
            if (controller?.Policy != null && dependency != null)
                controller.Policy.AddDependency(dependency.Policy);
            return controller;
        }

        public static IController DependsOn(this IController controller, IController dependency)
        {
            if (controller?.Policy != null && dependency != null)
                controller.Policy.AddDependency(dependency.Policy);
            return controller;
        }

        public static IHandler TrackPromise<P>(
            this IHandler handler, IPolicyOwner<P> policyOwner)
            where P : IPolicy
        {
            return policyOwner == null ? handler
                 : TrackPromise(handler, policyOwner.Policy);
        }

        public static IHandler TrackPromise(
            this IHandler handler, IPolicy parentPolicy)
        {
            if (parentPolicy == null) return handler;
            return handler.Filter((callback, composer, proceed) => {
                var handled = proceed();
                if (handled)
                {
                    var cb = callback as ICallback;
                    if (cb?.Result is Promise promise)
                    {
                        var dependency = new PromisePolicy(promise).AutoRelease();
                        parentPolicy.AddDependency(dependency);
                        promise.Finally(() => parentPolicy.RemoveDependency(dependency));
                    }
                }
                return handled;
           });
        }
    }
}
