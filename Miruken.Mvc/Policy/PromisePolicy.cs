using System;
using SixFlags.CF.Miruken.Concurrency;

namespace SixFlags.CF.Miruken.MVC.Policy
{
    public class PromisePolicy : DefaultPolicy
    {
        private readonly WeakReference _promise;

        public PromisePolicy(Promise promise)
        {
            if (promise == null)
                throw new ArgumentNullException("promise");
            _promise = new WeakReference(promise);
            Track();
        }

        public Promise Promise
        {
           get { return _promise.Target as Promise; }
        }

        public PromisePolicy AutoRelease()
        {
            AutoRelease(() =>
            {
                var promise = Promise;
                if (promise == null) return;
                promise.Cancel();
            });
            return this;
        }
    }
}
