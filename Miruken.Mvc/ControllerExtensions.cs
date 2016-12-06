using Miruken.Callback;
using Miruken.Concurrency;
using Miruken.Context;
using Miruken.Mvc.Policy;
using Miruken.Mvc.Views;

namespace Miruken.Mvc
{
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

        public static IController DoestNotDependOn(this IController controller, IView dependency)
        {
            if (controller?.Policy != null && dependency != null)
                controller.Policy.RemoveDependency(dependency.Policy);
            return controller;
        }

        public static IController DependsOn(this IController controller, IController dependency)
        {
            if (controller?.Policy != null && dependency != null)
                controller.Policy.AddDependency(dependency.Policy);
            return controller;
        }

        public static bool DoesDependOn(this IController controller, IView dependency)
        {
            if (controller?.Policy != null && dependency != null)
                return controller.Policy.IsDependency(dependency.Policy);
            return false;
        }

        public static IHandler PublishFromRoot(this IHandler handler)
        {
            var context = handler.Resolve<IContext>();
            return context != null ? context.Root.Publish() : handler;
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
                    var promise = cb?.Result as Promise;
                    if (promise != null)
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
