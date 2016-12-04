using System;
using System.Windows.Forms;
using SixFlags.CF.Miruken.Callback;
using SixFlags.CF.Miruken.Concurrency;

namespace SixFlags.CF.Miruken.MVC
{
    public static class ConcurrencyExtensions
    {
        public static ICallbackHandler MainThread(this ICallbackHandler handler)
        {
            return handler.Filter((callback, composer, proceed) =>
                Main.InvokeRequired ? Equals(Main.Invoke(proceed), true)
                                    : proceed());
        }

        public static Promise MainThread(this Promise promise)
        {
            return promise.Decorate(resolve => (result, s) => {
                if (Main.InvokeRequired)
                {
                    Main.Invoke(resolve, result, s);
                    return;
                }
                resolve(result, s);
            }, reject => (ex, s) => {
                if (Main.InvokeRequired)
                {
                    Main.Invoke(reject, ex, s);
                    return;
                }
                reject(ex, s);             
            });          
        }

        public static Promise<T> MainThread<T>(this Promise<T> promise)
        {
            return promise.Decorate(resolve => (result, s) =>
            {
                if (Main.InvokeRequired)
                {
                    Main.Invoke(resolve, result, s);
                    return;
                }
                resolve(result, s);
            }, reject => (ex, s) =>
            {
                if (Main.InvokeRequired)
                {
                    Main.Invoke(reject, ex, s);
                    return;
                }
                reject(ex, s);
            });          
        }

        public static Action MainThread(this Action action)
        {
            return action == null ? (Action)null : () =>
            {
                if (Main.InvokeRequired)
                    Main.Invoke(action);
                else
                    action();
            };
        }

        public static Action<T> MainThread<T>(this Action<T> action)
        {
            return action == null ? (Action<T>)null : arg => {
                if (Main.InvokeRequired)
                    Main.Invoke(action, arg);
                else
                    action(arg);
            };
        }

        static readonly Control Main = new Control();
    }
}
