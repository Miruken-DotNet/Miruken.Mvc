namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Concurrency;
    using Miruken.Container;
    using Miruken.Context;
    using Miruken.Mvc.Options;
    using Miruken.Mvc.Views;

    public class MVCHandler : CompositeCallbackHandler, IMVC
    {
        public MVCHandler(IViewRegion mainRegion)
        {
            AddHandlers(mainRegion);
        }

        V IMVC.View<V>()
        {
            var composer   = Composer;
            var controller = ResolveViewController(composer, typeof(V));
            var container  = new IContainer(composer);
            var view       = container.Resolve<V>();
            view.Controller = controller;
            view.Policy.OnRelease(() => container.Release(view));
            view.Policy.Track();
            return view;
        }

        V IMVC.View<V>(Action<V> init)
        {
            var view = ((IMVC)this).View<V>();
            if (init != null) init(view);
            return view;
        }

        IViewLayer IMVC.ShowView<V>()
        {
            return ObtainAndShow<V>(null);
        }

        IViewLayer IMVC.ShowView<V>(V view)
        {
            return Show(view);
        }

        IViewLayer IMVC.ShowView<V>(Action<V> init)
        {
            return ObtainAndShow(init);
        }

        Promise<IContext> IMVC.Next<C>(Action<C> action)
        {
            return ((IMVC)this).Nav(action, NavigationStyle.Next);
        }

        Promise<IContext> IMVC.Push<C>(Action<C> action)
        {
            return ((IMVC)this).Nav(action, NavigationStyle.Push);
        }

        Promise<IContext> IMVC.Part<C>(Action<C> action)
        {
            return ((IMVC)this).Nav(action, NavigationStyle.Part);
        }

        Promise<IContext> IMVC.Nav<C>(Action<C> action, NavigationStyle navStyle)
        {
            var composer = Composer;
            if (action == null || composer == null) return null;

            var context   = composer.Resolve<IContext>();
            var initiator = composer.Resolve<IController>();

            var ctx = navStyle != NavigationStyle.Next
                    ? context.CreateChild()
                    : context;

            C controller;
            try
            {
                controller = (C)ResolveController(ctx, typeof(C));
                if (initiator != null && initiator.Context != ctx)
                    initiator.DependsOn(controller);
            }
            catch (Exception exception)
            {
                if (navStyle != NavigationStyle.Next)
                    ctx.End();
                return Promise<IContext>.Rejected(exception);
            }

            var oldIO = Controller._io;

            try
            {
                if (navStyle == NavigationStyle.Push)
                    composer = composer.PushLayer();
                else
                {
                    var ctrl = controller as Controller;
                    var init = initiator as Controller;
                    if (ctrl != null && init != null)
                    {
                        ctrl._lastAction  = h => new IMVC(h).Next(action);
                        ctrl._retryAction = init._lastAction;
                    }
                }

                // Propogate handler options
                Controller._io = ctx.Chain(composer);

                action(controller);

                if (initiator != null && initiator.Context == ctx)
                    initiator.Release();
            }
            catch (Exception exception)
            {
                if (navStyle != NavigationStyle.Next)
                    ctx.End();
                else if (initiator != null && initiator.Context == ctx)
                    controller.DependsOn(initiator);
                return Promise<IContext>.Rejected(exception);
            }
            finally
            {
                Controller._io = oldIO;
            }

            return Promise.Resolved(ctx);
        }

        Promise<IContext> IMVC.GoBack()
        {
            var composer = Composer;
            if (composer == null) return null;
            var controller = composer.Resolve<Controller>();
            return (controller != null && controller._retryAction != null)
                 ? controller._retryAction(composer)
                 : null;
        }

        private IViewLayer ObtainAndShow<V>(Action<V> init)
            where V : IView
        {
            var view = ((IMVC)this).View(init);
            return Show(view);
        }

        private static IViewLayer Show(IView view)
        {
            return view.Display(new IViewRegion(Composer));
        }

        private static IController ResolveViewController(
            ICallbackHandler handler, Type viewType)
        {
            var controllerType = viewType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IView<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault() ?? typeof(IController);
            var controller = handler.Resolve(controllerType) as IController;
            if (controller == null)
                throw new InvalidOperationException(string.Format(
                    "Unable to find a compatible controller for {0}",
                    viewType.Name));
            return controller;
        }

        private static IController ResolveController(IContext context, Type type)
        {
            var controller = (IController)new IContainer(context).Resolve(type);
            context.ContextEnded += _ => controller.Release();
            controller.Policy.AutoRelease();
            controller.Context = context;
            return controller;
        }
    }
}
