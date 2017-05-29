using System;
using Miruken.Concurrency;

namespace Miruken.Mvc.Policy
{
    public class PromisePolicy : DefaultPolicy
    {
        private readonly WeakReference _promise;

        public PromisePolicy(Promise promise)
        {
            if (promise == null)
                throw new ArgumentNullException(nameof(promise));
            _promise = new WeakReference(promise);
            Track();
        }

        public Promise Promise => _promise.Target as Promise;

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
