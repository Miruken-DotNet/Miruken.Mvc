using System;
using SixFlags.CF.Miruken.MVC.Policy;

namespace SixFlags.CF.Miruken.MVC.Views
{
    public class ViewPolicy : DefaultPolicy
    {
        private readonly WeakReference _view;

        public ViewPolicy(IView view)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            _view = new WeakReference(view);
        }

        public IView View
        {
            get { return _view.Target as IView; }
        }
    }
}
