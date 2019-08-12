namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Callback.Policy.Bindings;
    using Concurrency;
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
        public Promise<Context> Navigate(Navigation navigation,
            Context context, IHandler composer)
        {
            var parent    = context;
            var initiator = context.Self().Resolve<Navigation>();
            var options   = composer.GetOptions(new NavigationOptions());
            var style     = options?.GoBack == true ? NavigationStyle.Next : navigation.Style;

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

                navigation.ViewRegion = initiator.ViewRegion;

                if (style != NavigationStyle.Push)
                {
                    parent = parent.Parent ?? throw new InvalidOperationException(
                                 "Navigation seems to be in a bad state");
                    navigation.ViewLayer = initiator.ViewLayer;
                }
            }

            IController controller = null;
            var child = parent.CreateChild();

            if (style == NavigationStyle.Push)
                child = child.CreateChild();

            try
            {
                controller = GetController(child, navigation.ControllerKey, composer);
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

            navigation.NoBack = options?.NoBack == true || options?.GoBack == true;
            if (!navigation.NoBack && navigation.Back == null && initiator != null &&
                style == NavigationStyle.Next)
            {
                navigation.Back = initiator;
            }

            if (style == NavigationStyle.Next)
                navigation.Back = initiator;

            BindIO(child, controller, style, options, composer);
            child.AddHandlers(navigation);

            return new Promise<Context>(ChildCancelMode.Any, (resolve, reject) =>
            {
                try
                {
                    child.ContextEnding += (ctx, reason) =>
                    {
                        if (reason is NavigationException exception)
                            reject(exception, true);
                        else if (!(reason is Navigation))
                            resolve(ctx, true);
                    };
                    if (style == NavigationStyle.Push)
                    {
                        child.Parent.ChildContextEnded += (ctx, reason) =>
                        {
                            if (reason is NavigationException exception)
                            {
                                ctx.Parent?.End(reason);
                                reject(exception, true);
                            }
                            else if (!(reason is Navigation))
                            {
                                ctx.Parent?.End(reason);
                                resolve(ctx, true);
                            }
                        };
                    }
                    if (!navigation.InvokeOn(controller, args =>
                    {
                        var values = child.ResolveArgs(args);
                        if (values != null)
                        {
                            if (style != NavigationStyle.Push)
                                initiator?.Context?.End(initiator);
                        }
                        return values;
                    }))
                    {
                        var exception = new NavigationException(context,
                            "Navigation could not be performed.  The most likely cause is missing dependencies.");
                        reject(exception, true);
                        child.End(exception);
                    }
                }
                catch (Exception ex)
                {
                    var exception = new NavigationException(context, "Navigation failed", ex);
                    reject(exception, true);
                    child.End(exception);
                }
                finally
                {
                    BindIO(null, controller, style);
                }
            });
        }

        [Handles]
        public Promise<Context> GoBack(Navigation.GoBack goBack, IHandler composer)
        {
            var navigation = composer.Resolve<Navigation>();
            if (navigation != null)
            {
                if (navigation.NoBack) return null;
                if (navigation.Style == NavigationStyle.Partial)
                {
                    if (navigation.Context != null)
                        FindNearest(navigation.Context, out navigation);
                }

                if (navigation == null) return null;

                if (navigation.Back != null)
                    return composer.NavBack().CommandAsync<Context>(navigation.Back);

                if (navigation.Style == NavigationStyle.Push)
                {
                    var context = navigation.Context;
                    if (context != null)
                    {
                        context.End();
                        return Promise.Resolved(context);
                    }
                }
            }

            return null;
        }

        private static void BindIO(IHandler io,  IController controller,
            NavigationStyle style, NavigationOptions options = null,
            IHandler composer = null)
        {
            var prepare = Navigation.GlobalPrepare;
            io = prepare?.GetInvocationList()
                .Cast<Navigation.Prepare>()
                .Aggregate(io ?? controller.Context,
                    (cur, b) => b(cur) ?? cur) ?? io;
            if (composer != null)
            {
                if (style == NavigationStyle.Push)
                {
                    if (options == null) options = new NavigationOptions();
                    if (style == NavigationStyle.Push)
                        (options.Region ?? (options.Region = new RegionOptions()))
                            .Push = true;
                }
                if (options != null)
                    io = io.Break().NavigationOptions(options);
            }
            controller.IO = io;
        }

        private static IController GetController(
            Context context, object key, IHandler composer)
        {
            var controller = (IController)context.Self()
                .Chain(composer).Infer().Resolve(key, constraints =>
                    constraints.Require(Qualifier.Of<ContextualAttribute>()));
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
