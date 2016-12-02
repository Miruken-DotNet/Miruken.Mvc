namespace Miruken.Mvc.Policy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Miruken.Infrastructure;

    public class DefaultPolicy : DisposableObject, IPolicy
    {
        private Action _onRelease;
        private readonly List<IPolicy> _dependencies;
        private int _releasing;

        public DefaultPolicy()
        {
            _dependencies = new List<IPolicy>();
            Reset();
        }

        public IPolicy Track()
        {
            IsTracked = true;
            return this;
        }

        public IPolicy Retain()
        {
            IsTracked = false;
            return this;
        }

        public IPolicy Parent { get; set; }

        public bool IsTracked { get; private set; }

        public IPolicy[] Dependencies
        {
            get { return _dependencies.ToArray(); }
        }

        public bool IsDependency(IPolicy policy)
        {
            return _dependencies.Contains(policy) ||
                _dependencies.Any(d => d.IsDependency(policy));
        }

        public virtual IPolicy AddDependency(IPolicy dependency)
        {
            if (dependency.Parent == this || dependency == this)
                return this;

            if (dependency.IsDependency(this))
                throw new InvalidOperationException("Cyclic dependency detected");

            if (dependency.Parent != null)
                dependency.Parent.RemoveDependency(dependency);

            if (!_dependencies.Contains(dependency))
                _dependencies.Add(dependency);
            dependency.Parent = this;
            return this;
        }

        public virtual IPolicy RemoveDependency(IPolicy dependency)
        {
            if (dependency.Parent != this) return this;
            _dependencies.Remove(dependency);
            dependency.Parent = null;
            return this;
        }

        public void Release()
        {
            if (Interlocked.CompareExchange(ref _releasing, 1, 0) != 0)
                return;

            if (Parent != null)
                Parent.RemoveDependency(this);

            if (IsTracked)
            {
                foreach (var dependency in Dependencies)
                    dependency.Release();

                var onRelease = _onRelease;
                if (onRelease != null) onRelease();

                Reset();
            }

            _releasing = 0;
        }

        public IDisposable OnRelease(Action onRelease)
        {
            _onRelease = (Action)Delegate.Combine(_onRelease, onRelease);
            return new DisposableAction(() => 
                _onRelease = (Action)Delegate.Remove(_onRelease, onRelease));
        }

        protected DefaultPolicy AutoRelease(Action doRelease)
        {
            var released = false;
            OnRelease(() =>
            {
                if (released) return;
                released = true;
                doRelease();
            });
            return this;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Release();
        }

        private void Reset()
        {
            Parent           = null;
            IsTracked        = false;
            _dependencies.Clear();
            _onRelease       = null;
            _releasing       = 0;
        }
    }
}
