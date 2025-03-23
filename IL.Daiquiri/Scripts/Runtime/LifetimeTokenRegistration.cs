using System;
using System.Collections.Generic;

namespace IL.Daiquiri
{
    public readonly struct LifetimeTokenRegistration : IDisposable, IEquatable<LifetimeTokenRegistration>
    {
        private readonly ICollection<IDisposable> _disposables;
        private readonly IDisposable _disposable;

        internal LifetimeTokenRegistration(ICollection<IDisposable> disposables, IDisposable disposable)
        {
            _disposables = disposables;
            _disposable = disposable;
        }

        public override int GetHashCode()
        {
            var hashCode = (_disposables, _disposable).GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is LifetimeTokenRegistration registration && Equals(registration);
        }

        public bool Equals(LifetimeTokenRegistration registration)
        {
            var result = (_disposables, _disposable).Equals((registration._disposables, registration._disposable));

            return result;
        }

        public bool TryDeregister()
        {
            if (_disposables == null)
            {
                return false;
            }

            if (_disposable == null)
            {
                return false;
            }

            var result = _disposables.Remove(_disposable);

            return result;
        }

        void IDisposable.Dispose()
        {
            TryDeregister();
        }

        bool IEquatable<LifetimeTokenRegistration>.Equals(LifetimeTokenRegistration other)
        {
            return Equals(other);
        }
    }
}
