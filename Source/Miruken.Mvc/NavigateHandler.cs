namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Container;
    using Context;
    using Options;
    using Views;

    public class NavigateHandler : CompositeHandler, INavigate
    {
        public NavigateHandler(IViewRegion mainRegion)
        {
            AddHandlers(mainRegion);
        }

        object INavigate.Next<C>(Func<C, object> action)
        {
            return Navigate(action, NavigationStyle.Next);
        }

        object INavigate.Push<C>(Func<C, object> action)
        {
            return Navigate(action, NavigationStyle.Push);
        }

        object INavigate.Navigate<C>(Func<C, object> action, NavigationStyle style)
        {
            return Navigate(action, style);
        }

        private static object Navigate<C>(Func<C, object> action, NavigationStyle style)
            where C : class, IController
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var composer = Composer;
            var context  = composer?.Resolve<IContext>();
            if (context == null)
                throw new InvalidOperationException(
                    "A context is required for controller navigation");

            var initiator = composer.Resolve<IController>();

            var ctx = style != NavigationStyle.Next
                    ? context.CreateChild()
                    : context;

            C controller;
            try
            {
                controller = (C)ResolveController(ctx, typeof(C));
                if (initiator != null && initiator != controller &&
                    initiator.Context != ctx)
                    initiator.DependsOn(controller);
            }
            catch
            {
                if (style != NavigationStyle.Next) ctx.End();
                throw;
            }

            var ctrl = controller as Controller;

            try
            {
                if (style == NavigationStyle.Push)
                    composer = composer.PushLayer();
                else
                {
                    var init = initiator as Controller;
                    if (ctrl != null)
                    {
                        ctrl._lastAction  = h => h.Proxy<INavigate>().Next(action);
                        if (init != null)
                            ctrl._retryAction = init._lastAction;
                    }
                }

                var oldIo = ctrl?._io;

                if (ctrl != null)
                {
                    // Propogate composer options
                    var io = ReferenceEquals(context, ctx)
                           ? composer : ctx.Self().Chain(composer);
                    var prepare = Controller.GlobalPrepare;
                    if (prepare != null)
                    {
                        io = prepare.GetInvocationList().Cast<FilterBuilder>()
                            .Aggregate(io, (cur, b) => b(cur) ?? cur);
                    }
                    ctrl._io = io;
                }

                try
                {
                    return action(controller);
                }
                finally
                {
                    if (ctrl != null) ctrl._io = oldIo;
                    if (initiator != null && initiator.Context == ctx &&
                        initiator != controller)
                    {
                        initiator.Release();
                        initiator.Context = null;
                    }
                }
            }
            catch
            {
                if (style != NavigationStyle.Next)
                    ctx.End();
                else if (initiator != null && initiator.Context == ctx)
                    controller.DependsOn(initiator);
                throw;
            }
        }

        object INavigate.GoBack()
        {
            var composer   = Composer;
            var controller = composer?.Resolve<Controller>();
            return controller?._retryAction?.Invoke(composer);
        }

        private static IController ResolveController(IContext context, Type type)
        {
            var controller = (IController)context.Proxy<IContainer>().Resolve(type);
            if (controller == null)
                throw new NotSupportedException($"Controller {type.FullName} could not be resolved");
            context.ContextEnded += _ => controller.Release();
            controller.Policy.AutoRelease();
            controller.Context = context;
            return controller;
        }
    }
}
