using System;
using Miruken.Mvc.Policy;

namespace Miruken.Mvc
{
    public class ControllerPolicy : DefaultPolicy
    {
        private readonly WeakReference _controller;

        public ControllerPolicy(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            _controller = new WeakReference(controller);
            Track();
        }

        public IController Controller => _controller.Target as IController;
    }
}
