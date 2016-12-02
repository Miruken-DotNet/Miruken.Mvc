namespace Miruken.Mvc.Policy
{
    using System;

    public interface IPolicy : IDisposable
    {
        IPolicy Track();

        IPolicy Retain();

        bool IsTracked { get; }

        IPolicy Parent { get; set; }

        IPolicy[] Dependencies { get; }

        bool IsDependency(IPolicy policy);

        IPolicy AddDependency(IPolicy policy);

        IPolicy RemoveDependency(IPolicy policy);

        IDisposable OnRelease(Action policy);

        void Release();
    }

    public interface IPolicyOwner<P> where P : IPolicy
    {
        P Policy { get; }
    }
}
