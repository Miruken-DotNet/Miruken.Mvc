using System;
using Miruken.Callback;
using Miruken.Container;
using Miruken.Context;
using Miruken.Mvc;
using Miruken.Mvc.Options;
using Miruken.Mvc.Views;
using static Miruken.Protocol;

namespace Miruken.MVC
{
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
            where C : IController
        {
            if (action == null) return null;

            var composer  = Composer;
            var context   = composer?.Resolve<IContext>();
            if (context == null)
                throw new InvalidOperationException(
                    "A context is required for navigation");

            var initiator = composer.Resolve<IController>();

            var ctx = style != NavigationStyle.Next
                    ? context.CreateChild()
                    : context;

            C controller;
            try
            {
                controller = (C)ResolveController(ctx, typeof(C));
                if (initiator != null && initiator.Context != ctx)
                    initiator.DependsOn(controller);
            }
            catch
            {
                if (style != NavigationStyle.Next)
                    ctx.End();
                throw;
            }

            var oldIO = Controller._io;

            try
            {
                if (style == NavigationStyle.Push)
                    composer = composer.PushLayer();
                else
                {
                    var ctrl = controller as Controller;
                    var init = initiator as Controller;
                    if (ctrl != null && init != null)
                    {
                        ctrl._lastAction  = h => P<INavigate>(h).Next(action);
                        ctrl._retryAction = init._lastAction;
                    }
                }

                // Propogate composer options
                Controller._io = ctx.Chain(composer);

                try
                {
                    return action(controller);
                }
                finally 
                {
                    if (initiator != null && initiator.Context == ctx)
                        initiator.Release();
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
            finally
            {
                Controller._io = oldIO;
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
            var controller = (IController)P<IContainer>(context).Resolve(type);
            context.ContextEnded += _ => controller.Release();
            controller.Policy.AutoRelease();
            controller.Context = context;
            return controller;
        }
    }
}
