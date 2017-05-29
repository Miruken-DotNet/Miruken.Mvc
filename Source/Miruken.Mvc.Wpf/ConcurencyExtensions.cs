using System;
using System.Windows;
using Miruken.Callback;
using Miruken.Concurrency;

namespace Miruken.Mvc.Wpf
{
    public static class ConcurrencyExtensions
    {
        public static IHandler MainThread(this IHandler handler)
        {
            var dispatcher = Application.Current.Dispatcher;
            return handler.Filter((callback, composer, proceed) =>
                dispatcher.CheckAccess() ? proceed() : Equals(dispatcher.Invoke(proceed), true));
        }

        public static Promise MainThread(this Promise promise)
        {
            var dispatcher = Application.Current.Dispatcher;
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
            var dispatcher = Application.Current.Dispatcher;
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
            var dispatcher = Application.Current.Dispatcher;
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
            var dispatcher = Application.Current.Dispatcher;
            return action == null ? (Action<T>)null : arg => {
                if (!dispatcher.CheckAccess())
                    dispatcher.Invoke(action, arg);
                else
                    action(arg);
            };
        }
    }
}
