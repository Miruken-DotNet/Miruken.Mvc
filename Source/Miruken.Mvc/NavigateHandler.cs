namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Options;
    using Views;

    public class NavigateHandler : CompositeHandler
    {
        public NavigateHandler(IViewRegion mainRegion)
        {
            AddHandlers(mainRegion);
        }

        [Handles]
        public object Navigate(Navigation navigation, IHandler composer)
        {
            var context  = composer?.Resolve<Context>();
            if (context == null)
            {
                throw new InvalidOperationException(
                    "A context is required for controller navigation");
            }

            var style            = navigation.Style;
            var initiator        = context.SelfOrChild().Resolve<Navigation>();
            var initiatorContext = initiator?.Controller?.Context;
            var parentContext    = context;

            if (initiator != null && style == NavigationStyle.Next)
            {
                parentContext = initiatorContext?.Parent;
                if (parentContext == null) return null;
            }

            IController controller = null;
            var childContext = parentContext.CreateChild();
            try
            {
                controller = GetController(childContext, navigation.ControllerType);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (controller == null)
                    childContext.End();
            }

            childContext.AddHandlers(navigation);

            if (initiator != null && style == NavigationStyle.Next)
                initiatorContext?.End();

            try
            {
                if (style == NavigationStyle.Push)
                    composer = composer.PushLayer();
                else
                    navigation.Back = initiator;

                // Propagate composer options
                var io = childContext.Self().Chain(composer);
                BindIO(io, controller);

                try
                {
                    navigation.InvokeOn(controller);
                }
                finally
                {
                    BindIO(null, controller);
                }
            }
            catch
            {
                childContext.End();
            }

            return true;
        }

        [Handles]
        public object GoBack(GoBack goBack, IHandler composer)
        {
            var back = composer.Resolve<Navigation>()?.Back;
            if (back != null && composer.Handle(back))
            {
                goBack.SetResult(back.ClearResult());
                return true;
            }
            return null;
        }

        private static void BindIO(IHandler io, IController controller)
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
            controller.IO = io;
        }

        private static IController GetController(Context context, Type type)
        {
            var controller = (IController)context.Resolve(type);
            if (controller == null) return null;
            context.ContextEnded += _ => controller.Release();
            controller.Context = context;
            return controller;
        }
    }
}
