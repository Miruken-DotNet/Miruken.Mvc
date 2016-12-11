using System;
using Miruken.Callback;
using Miruken.Container;

namespace Miruken.Mvc.Tests
{
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

        T[] IContainer.ResolveAll<T>()
        {
            return Unhandled<T[]>();
        }

        object[] IContainer.ResolveAll(object key)
        {
            return Unhandled<object[]>();
        }

        void IContainer.Release(object component)
        {
        }
    }
}
