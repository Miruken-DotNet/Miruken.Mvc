namespace Miruken.Mvc
{
    using System;
    using Miruken.Container;
    using Miruken.Mvc.Policy;

    public class ControllerPolicy : DefaultPolicy
    {
        private readonly WeakReference _controller;

        public ControllerPolicy(IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            _controller = new WeakReference(controller);
            Track();
        }

        public IController Controller
        {
           get { return _controller.Target as IController; }
        }

        public ControllerPolicy AutoRelease()
        {
            AutoRelease(() =>
            {
                var controller = Controller;
                if (controller == null) return;
                var context = controller.Context;
                if (context != null)
                    new IContainer(context).Release(controller);
            });
            return this;
        }
    }
}
