namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Error;
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
            var context  = composer?.Resolve<Context>();
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

                if (ctrl != null)
                {
                    // Propagate composer options
                    var io = ReferenceEquals(context, ctx)
                           ? composer : ctx.Self().Chain(composer);
                    BindIO(io, ctrl);
                }

                try
                {
                    return action(controller);
                }
                finally
                {
                    BindIO(null, ctrl);
                    if (initiator != null && initiator.Context == ctx &&
                        initiator != controller)
                    {
                        initiator.Release();
                        initiator.Context = null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (style != NavigationStyle.Next)
                    ctx.End();
                else if (initiator != null && initiator.Context == ctx)
                    controller.DependsOn(initiator);
                return ctrl?._io.Proxy<IErrors>().HandleException(ex);
            }
        }

        object INavigate.GoBack()
        {
            var composer   = Composer;
            var controller = composer?.Resolve<Controller>();
            return controller?._retryAction?.Invoke(composer);
        }

        private static void BindIO(IHandler io, Controller controller)
        {
            if (controller == null) return;
            var prepare = Controller.GlobalPrepare;
            if (prepare != null)
            {
                io = prepare.GetInvocationList()
                    .Cast<FilterBuilder>()
                    .Aggregate(io ?? controller.Context,
                        (cur, b) => b(cur) ?? cur);
            }
            controller._io = io;
        }

        private static IController ResolveController(Context context, Type type)
        {
            var controller = (IController)context.Resolve(type);
            if (controller == null)
                throw new NotSupportedException(
                    $"Controller {type.FullName} could not be resolved");
            context.ContextEnded += _ => controller.Release();
            controller.Context = context;
            controller.Policy.AutoRelease();
            return controller;
        }
    }
}
