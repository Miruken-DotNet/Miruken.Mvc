﻿namespace Miruken.Mvc.Views
{
    using System;
    using Callback;
    using Context;
    using Options;

    /// <summary>
    /// View adapter that installs a new <see cref="IViewRegion"/>
    /// stack in a new child <see cref="Context"/> and pushes a
    /// new <see cref="C"/>controller on to the stack.
    /// </summary>
    /// <typeparam name="C">Concrete controller</typeparam>
    internal class RegionView<C> : ViewAdapter 
        where C : class, IController
    {
        private readonly Action<C> _action;
        private readonly IHandler _composer;

        public RegionView(Action<C> action, IHandler composer)
        {
            _action   = action;
            _composer = composer;
        }

        /// <summary>
        /// Displays a new controller in a new context as the
        /// top layer in a <see cref="IViewStackView"/>.  When
        /// the layer is destroyed, the associated controller's
        /// context is ended.
        /// </summary>
        /// <param name="region">The actual region to display in</param>
        /// <returns>The layer representing the new controller</returns>
        public override IViewLayer Display(IViewRegion region)
        {
            IViewLayer layer = null;
            var stack = region.CreateViewStack();
            _composer.Push((C controller) =>
            {
                var context = controller.Context;
                context.AddHandlers(stack);
                _action(controller);
                layer = stack.Display(_composer.PushLayer().Proxy<IViewRegion>());
                layer.Disposed += (s, e) => context.End();
                context.ContextEnded += (ctx, _) => layer.Dispose();
            });
            return layer;
        }
    }

    public static class RegionExtensions
    {
        public static IView Region<C>(this IHandler handler,
            Action<C> action) where C : class, IController
        {
            return new RegionView<C>(action, handler);
        }

        public static Context AddRegion(this Context context, IViewRegion region)
        {
            var child = context.CreateChild();
            child.AddHandlers(region);
            return child;
        }
    }
}
