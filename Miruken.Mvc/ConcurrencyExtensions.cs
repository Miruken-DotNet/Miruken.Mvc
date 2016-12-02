namespace Miruken.Mvc
{
    using System;
    using System.Windows.Threading;
    using Callback;
    using Concurrency;

    public static class ConcurrencyExtensions
    {
        public static ICallbackHandler MainThread(this ICallbackHandler handler)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            return handler.Filter((callback, composer, proceed) =>
                dispatcher.CheckAccess() ? proceed() : Equals(dispatcher.Invoke(proceed), true));
        }

        public static Promise MainThread(this Promise promise)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            return promise.Decorate(resolve => (result, s) => {
                if (!dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(resolve, result, s);
                    return;
                }
                resolve(result, s);
            }, reject => (ex, s) => {
                if (!dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(reject, ex, s);
                    return;
                }
                reject(ex, s);             
            });          
        }

        public static Promise<T> MainThread<T>(this Promise<T> promise)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            return promise.Decorate(resolve => (result, s) =>
            {
                if (!dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(resolve, result, s);
                    return;
                }
                resolve(result, s);
            }, reject => (ex, s) =>
            {
                if (!dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(reject, ex, s);
                    return;
                }
                reject(ex, s);
            });          
        }

        public static Action MainThread(this Action action)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            return action == null ? (Action)null : () =>
            {
                if (!dispatcher.CheckAccess())
                    dispatcher.Invoke(action);
                else
                    action();
            };
        }

        public static Action<T> MainThread<T>(this Action<T> action)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;
            return action == null ? (Action<T>)null : arg => {
                if (!dispatcher.CheckAccess())
                    dispatcher.Invoke(action, arg);
                else
                    action(arg);
            };
        }
        
    }
}
