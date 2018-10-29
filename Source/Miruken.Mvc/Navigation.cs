﻿namespace Miruken.Mvc
{
    using System;
    using Options;

    public enum NavigationStyle
    {
        Next,
        Push,
        Partial
    }

    public class Navigation
    {
        private WeakReference<IController> _controller;

        public Navigation(
            Type controllerType,
            Action<IController> action,
            NavigationStyle style,
            RegionOptions options = null)
        {
            ControllerType = controllerType
                ?? throw new ArgumentNullException(nameof(controllerType));
            Action = action;
            Action         = action
                ?? throw new ArgumentNullException(nameof(action));
            Style          = style;
            Options        = options;
        }

        public Type                ControllerType { get; }
        public Action<IController> Action         { get; }
        public NavigationStyle     Style          { get; }
        public RegionOptions       Options        { get; }
        public Navigation          Back           { get; set; }

        public IController Controller => 
            _controller != null && 
            _controller.TryGetTarget(out var controller)
                ? controller : null;

        public void InvokeOn(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            if (!(ControllerType.IsInstanceOfType(controller)))
                throw new ArgumentException(
                    $"{controller} is not a {ControllerType.FullName}");
            _controller = new WeakReference<IController>(controller);
            Action(controller);
        }

        public class GoBack
        {
            public GoBack(RegionOptions options = null)
            {
                Options = options;
            }

            public RegionOptions Options { get; }
        }
    }
}