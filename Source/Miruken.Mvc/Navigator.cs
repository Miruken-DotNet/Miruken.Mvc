namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Graph;
    using Options;
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

                if (style != NavigationStyle.Push)
                    parent = parent.Parent ?? throw new InvalidOperationException(
                                 "Navigation seems to be in a bad state");
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

            BindIO(child, controller, style, composer);
            child.AddHandlers(navigation);

            try
            {

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
                BindIO(null, controller, style, null);
            }

            return true;
        }

        [Handles]
        public object GoBack(Navigation.GoBack goBack, IHandler composer)
        {
            var navigation = composer.Resolve<Navigation>();
            if (navigation != null)
            {
                if (navigation.Back != null)
                    return composer.Handle(navigation.Back);
                if (navigation.Style == NavigationStyle.Push)
                {
                    var controller = navigation.Controller;
                    if (controller != null)
                    {
                        controller.Context?.End(controller);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void BindIO(IHandler io, 
            IController controller, NavigationStyle style,
            IHandler composer)
        {
            var prepare = Controller.GlobalPrepare;
            io = prepare?.GetInvocationList()
                .Cast<FilterBuilder>()
                .Aggregate(io ?? controller.Context,
                    (cur, b) => b(cur) ?? cur) ?? io;
            if (composer != null)
            {
                var options = new RegionOptions();
                if (composer.Handle(options, true) ||
                    style == NavigationStyle.Push)
                {
                    if (style == NavigationStyle.Push)
                        (options.Layer ?? (options.Layer = new LayerOptions()))
                            .Push = true;
                    io = io.Break().RegionOptions(options);
                }
            }
            controller.IO = io;
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
