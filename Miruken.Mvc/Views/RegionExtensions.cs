using System;
using Miruken.Callback;
using Miruken.Context;
using Miruken.MVC;
using static Miruken.Protocol;

namespace Miruken.Mvc.Views
{
    abstract class RegionViewAdapter : ViewAdapter
    {
        /// <summary>
        /// Region Adapter to capture the controllers <see cref="IView"/>
        /// and display it in the newly created <see cref="IViewStackView"/>.
        /// That stack view is then displayed in the actual region.
        /// </summary>
        protected class ViewStackAdapter : RegionAdapter, IViewStack
        {
            private readonly IViewStackView _stack;

            public ViewStackAdapter(IViewRegion region, IViewStackView stack)
				: base(region)
			{
			    _stack = stack;
			}

            public override IViewLayer Show(IView view)
            {
                var layer = view.Display(_stack);
				_stack.Display(Inner);
				return layer;
			}

            IDisposable IViewStack.PushLayer()
            {
                return _stack.PushLayer();
            }

            void IViewStack.UnwindLayers()
            {
                _stack.UnwindLayers();
            }
        }
    }

    /// <summary>
    /// View adapter that installs a new <see cref="IViewRegion"/>
    /// stack in a new child <see cref="IContext"/> and pushes a
    /// new <see cref="C"/>controller on to the stack.
    /// </summary>
    /// <typeparam name="C">Concrete controller</typeparam>
    class RegionView<C> : RegionViewAdapter where C : IController
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
            var stack = P<IViewRegion>(_composer).View<IViewStackView>();
            var stackAdapter = new ViewStackAdapter(region, stack);
            // Temporarily install the stack region adapter.
            P<INavigate>(new Handler(stackAdapter).Chain(_composer)).Push<C>(
                controller => {
                    stack.Controller = controller;
                    var context = controller.Context;
                    context.AddHandlers(stack);
                    _action(controller);
                    stackAdapter.ViewLayer.Disposed += (s,e) => context.End();
                });
            return (Layer = stackAdapter.ViewLayer);
        }
    }

    public static class RegionExtensions
    {
        public static IView Region<C>(this IHandler handler,
            Action<C> action) where C : IController
        {
            return new RegionView<C>(action, handler);
        }

        public static IContext AddRegion(this IContext context, IViewRegion region)
        {
            var child = context.CreateChild();
            child.AddHandlers(region);
            return child;
        }
    }
}
