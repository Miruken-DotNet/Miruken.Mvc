using System;
using SixFlags.CF.Miruken.Container;
using SixFlags.CF.Miruken.MVC.Policy;

namespace SixFlags.CF.Miruken.MVC
{
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
