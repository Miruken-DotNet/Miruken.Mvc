namespace Miruken.Mvc
{
    using System;

    public enum NavigationStyle
    {
        Next,
        Push
    }

    public class Navigation
    {
        private readonly Func<IController, object> _action;
        private WeakReference<IController> _controller;
        private object _result;

        public Navigation(
            Type controllerType,
            Func<IController, object> action,
            NavigationStyle style)
        {
            ControllerType = controllerType
                ?? throw new ArgumentNullException(nameof(controllerType));
            _action        = action
                ?? throw new ArgumentNullException(nameof(action));
            Style          = style;
        }

        public Type            ControllerType { get; }
        public NavigationStyle Style          { get; }
        public Navigation      Back           { get; set; }

        public IController Controller => 
            _controller != null && 
            _controller.TryGetTarget(out var controller)
                ? controller : null;

        public object ClearResult()
        {
            var result = _result;
            _result    = null;
            return result;
        }

        public object InvokeOn(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (!(ControllerType.IsInstanceOfType(controller)))
                throw new ArgumentException(
                    $"{controller} is not a {ControllerType.FullName}");
            _controller = new WeakReference<IController>(controller);
            return _result = _action(controller);
        }
    }
}