using System;
using Miruken.Mvc.Policy;

namespace Miruken.Mvc.Views
{
    public class ViewPolicy : DefaultPolicy
    {
        private readonly WeakReference _view;

        public ViewPolicy(IView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            _view = new WeakReference(view);
        }

        public IView View => _view.Target as IView;
    }
}
