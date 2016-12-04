using System;
using SixFlags.CF.Miruken.Callback;
using SixFlags.CF.Miruken.Context;

namespace SixFlags.CF.Miruken.MVC.Views
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
        private readonly ICallbackHandler _composer;

        public PartialView(Action<C> action, ICallbackHandler composer)
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
            var context       = new INavigate(new CallbackHandler(regionAdapter)
                                    .Chain(_composer))
                                    .Part(_action);
            Layer = regionAdapter.ViewLayer;
            EventHandler transitioned = null;
            transitioned = (s, e) => {
                var initiator = CallbackHandler.Composer.Resolve<IController>();
                if (initiator == null || initiator.Context == context) return;
                context.End();
                Layer.Transitioned -= transitioned;
            };
            Layer.Transitioned += transitioned;
            Layer.Disposed += (s, e) => context.End();
            return Layer;
        }
    }

    public static class PartialExtensions
    {
        public static IView Partial<C>(this ICallbackHandler handler,
            Action<C> action) where C : IController
        {
            return new PartialView<C>(action, handler);
        }
    }
}
