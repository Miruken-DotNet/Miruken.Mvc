namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Views;

    public class Navigator : CompositeHandler
    {
        public Navigator(IViewRegion mainRegion)
        {
            AddHandlers(mainRegion);
        }

        [Handles]
        public object Navigate(
            Navigation navigation,
            [Optional] Navigation initiator,
            Context context, IHandler composer)
        {
            var style            = navigation.Style;
            var initiatorContext = initiator?.Controller?.Context;
            var parentContext    = context;

            if (initiator != null && style != NavigationStyle.Push)
            {
                parentContext = initiatorContext?.Parent;
                if (parentContext == null) return null;
            }

            IController controller = null;
            var childContext = parentContext.CreateChild();
            try
            {
                controller = GetController(childContext, navigation.ControllerType);
                if (controller == null) return null;
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

            if (initiator != null && style == NavigationStyle.Next)
            {
                navigation.Back = initiator;
                initiatorContext?.End();
            }

            // Propagate options (i.e. animation)
            var io = childContext.Self().Chain(composer);
            if (style == NavigationStyle.Partial)
                io = io.Provide(navigation);
            else
                childContext.AddHandlers(navigation);

            BindIO(io, controller);

            try
            {
                navigation.InvokeOn(controller);
            }
            catch
            {
                childContext.End();
                throw;
            }
            finally
            {
                BindIO(null, controller);
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
            controller.IO = prepare?.GetInvocationList()
                .Cast<FilterBuilder>()
                .Aggregate(io ?? controller.Context,
                    (cur, b) => b(cur) ?? cur) ?? io;
        }

        private static IController GetController(Context context, Type type)
        {
            var controller = (IController)context.Infer().Resolve(type);
            if (controller == null) return null;
            controller.Context = context;
            return controller;
        }
    }
}
