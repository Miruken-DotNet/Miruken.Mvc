using System;
using Miruken.Callback;
using Miruken.Container;

namespace Miruken.Mvc.Tests
{
    using Concurrency;

    class TestContainer : Handler, IContainer
    {
        T IContainer.Resolve<T>()
        {
            return Activator.CreateInstance<T>();
        }

        object IContainer.Resolve(object key)
        {
            var type = key as Type;
            return type != null
                 ? Activator.CreateInstance(type)
                 : Unhandled<object>();
        }

        Promise<T> IContainer.ResolveAsync<T>()
        {
            var container = (IContainer)this;
            return Promise.Resolved(container.Resolve<T>());
        }

        Promise IContainer.ResolveAsync(object key)
        {
            var container = (IContainer)this;
            return Promise.Resolved(container.Resolve(key));
        }

        T[] IContainer.ResolveAll<T>()
        {
            return Array.Empty<T>();
        }

        object[] IContainer.ResolveAll(object key)
        {
            return Array.Empty<object>();
        }

        Promise<T[]> IContainer.ResolveAllAsync<T>()
        {
            var container = (IContainer)this;
            return Promise.Resolved(container.ResolveAll<T>());
        }

        Promise<object[]> IContainer.ResolveAllAsync(object key)
        {
            var container = (IContainer)this;
            return Promise.Resolved(container.ResolveAll(key));
        }

        void IContainer.Release(object component)
        {
        }
    }
}
