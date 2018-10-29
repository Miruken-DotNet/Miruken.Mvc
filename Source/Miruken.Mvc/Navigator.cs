namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Graph;
    using Views;

    public class Navigator : CompositeHandler
    {
        public Navigator(IViewRegion mainRegion)
        {
            AddHandlers(mainRegion);
        }

        [Handles]
        public object Navigate(Navigation navigation,
            Context context, IHandler composer)
        {
            var style     = navigation.Style;
            var initiator = context.Self().Resolve<Navigation>();
            var parent    = context;

            if (initiator != null)
            {
                if (initiator.Style == NavigationStyle.Partial &&
                    navigation.Style != NavigationStyle.Partial)
                {
                    parent = FindNearest(context, out initiator);
                    if (parent == null)
                    {
                        throw new InvalidOperationException(
                            "Navigation from a partial requires a parent");
                    }
                }

                if (navigation.Style != NavigationStyle.Push)
                    parent = parent.Parent;
            }

            IController controller = null;
            var child = parent.CreateChild();
            try
            {
                controller = GetController(child, navigation.ControllerType);
                if (controller == null) return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (controller == null)
                    child.End();
            }

            if (style == NavigationStyle.Next)
                navigation.Back = initiator;

            BindIO(child, controller);

            try
            {
                child.AddHandlers(navigation);
                navigation.InvokeOn(controller);
                if (style != NavigationStyle.Push)
                    initiator?.Controller?.Context?.End(initiator);
            }
            catch
            {
                child.End();
                throw;
            }
            finally
            {
                BindIO(null, controller);
            }

            return true;
        }

        [Handles]
        public object GoBack(Navigation.GoBack goBack, IHandler composer)
        {
            var back = composer.Resolve<Navigation>()?.Back;
            if (back != null)
            {
                var navigation = new Navigation(
                    back.ControllerType, back.Action, back.Style,
                    goBack.Options);
                return composer.Handle(navigation);
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

        private static Context FindNearest(Context context, out Navigation initiator)
        {
            Context nearest = null;
            Navigation navigation = null;
            context.Traverse(TraversingAxis.Ancestor, node =>
            {
                var ctx = (Context)node;
                var nav = ctx.Self().Resolve<Navigation>();
                if (nav == null || nav.Controller?.Context != ctx ||
                    nav.Style == NavigationStyle.Partial) return false;
                navigation = nav;
                nearest    = ctx;
                return true;
            });
            initiator = navigation;
            return nearest;
        }
    }
}
