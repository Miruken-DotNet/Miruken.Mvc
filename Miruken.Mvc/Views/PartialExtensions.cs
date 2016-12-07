using System;
using Miruken.Callback;
using Miruken.Context;
using Miruken.MVC;
using static Miruken.Protocol;

namespace Miruken.Mvc.Views
{
    /// <summary>
    /// View adapter thats pushes a new <see cref="C"/>
    /// controller in a new child <see cref="IContext"/>
    /// on to the current <see cref="IViewRegion"/>.
    /// </summary>
    /// <typeparam name="C">Concrete controller</typeparam>
    public class PartialView<C> : ViewAdapter where C : IController
    {
        private readonly Action<C> _action;
        private readonly IHandler _composer;

        public PartialView(Action<C> action, IHandler composer)
        {
            _action   = action;
            _composer = composer;
        }

        /// <summary>
        /// Replaces a new controller in a new context at the
        /// current <see cref="IViewRegion"/> layer.  Subsequent
        /// transitions initiated by controllers in a differernt
        /// context from the new controller will end the new
        /// controller's context.  When the layer is destroyed,
        /// the new controller's context is ended.
        /// </summary>
        /// <param name="region">The actual region to display in</param>
        /// <returns>The active layer</returns>
        public override IViewLayer Display(IViewRegion region)
        {
            var regionAdapter = new RegionAdapter(region);
            var context       = P<INavigate>(new Handler(regionAdapter)
                                    .Chain(_composer))
                                    .Part(_action);
            Layer = regionAdapter.ViewLayer;
            context.Then((ctx, _) =>
            {
                EventHandler transitioned = null;
                transitioned = (s, e) =>
                {
                    var initiator = Handler.Composer.Resolve<IController>();
                    if (initiator == null || initiator.Context == ctx) return;
                    context.End();
                    Layer.Transitioned -= transitioned;
                };
                Layer.Transitioned += transitioned;
                Layer.Disposed += (s, e) => ctx.End();
            });
            return Layer;
        }
    }

    public static class PartialExtensions
    {
        public static IView Partial<C>(this IHandler handler,
            Action<C> action) where C : IController
        {
            return new PartialView<C>(action, handler);
        }
    }
}
