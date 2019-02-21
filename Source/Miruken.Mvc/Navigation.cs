namespace Miruken.Mvc
{
    using System;
    using System.Linq;
    using Callback;
    using Context;
    using Views;

    public enum NavigationStyle
    {
        Next,
        Push,
        Partial
    }

    public abstract class Navigation : IFilterCallback
    {
        public class GoBack
        {
        }

        public delegate IHandler Prepare(IHandler handler);

        public delegate bool Execute(Navigation navigation);

        private WeakReference<IController> _controller;
        private WeakReference<IViewLayer> _viewLayer;

        protected Navigation(object controllerKey, NavigationStyle style)
        {
            ControllerKey = controllerKey
                          ?? throw new ArgumentNullException(nameof(controllerKey));
            Style = style;
        }

        public object          ControllerKey { get; }
        public NavigationStyle Style         { get; }
        public bool            NoBack        { get; set; }
        public Navigation      Back          { get; set; }

        bool IFilterCallback.CanFilter => false;

        public Context Context => Controller?.Context;

        public IController Controller
        {
            get => _controller != null &&
                   _controller. TryGetTarget(out var controller)
                        ? controller : null;
            protected set => _controller = value == null ? null
                : new WeakReference<IController>(value);
        }

        public IViewLayer ViewLayer
        {
            get => _viewLayer != null &&
                   _viewLayer.TryGetTarget(out var viewLayer)
                        ? viewLayer : null;
            set => _viewLayer = value == null ? null 
                : new WeakReference<IViewLayer>(value);
        }

        public abstract bool InvokeOn(IController controller);
      
        public static Prepare GlobalPrepare;
        public static Execute GlobalExecute;
    }

    public class Navigation<C> : Navigation
        where C : IController
    {
        public Navigation(TargetAction<C> action, NavigationStyle style)
            : this(typeof(C), action, style)
        {
        }

        public Navigation(object controllerKey,
            TargetAction<C> action, NavigationStyle style)
            : base(controllerKey, style)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public TargetAction<C> Action { get; }

        public override bool InvokeOn(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (!(controller is C ctrl))
                throw new ArgumentException(
                    $"{controller} is not a {typeof(C).FullName}");
            var context = controller.Context ??
                throw new InvalidOperationException(
                    "Controller invocation requires a context");
            Controller = controller;
            return GlobalExecute?.GetInvocationList()
                       .All(ex => ((Execute)ex)(this)) != false
                   && Action(ctrl, context);
        }
    }
}