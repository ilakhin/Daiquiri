using System;
using System.Collections.Generic;

namespace IL.Daiquiri.Core
{
    public readonly struct LifetimeToken : IEquatable<LifetimeToken>
    {
        private readonly LifetimeTokenSource _source;

        internal LifetimeToken(LifetimeTokenSource source)
        {
            _source = source;
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<LifetimeTokenSource>.Default;
            var hashCode = comparer.GetHashCode(_source);

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is LifetimeToken other && Equals(other);
        }

        public bool Equals(LifetimeToken other)
        {
            var comparer = EqualityComparer<LifetimeTokenSource>.Default;
            var result = comparer.Equals(_source, other._source);

            return result;
        }

        public LifetimeTokenRegistration Register(IDisposable disposable)
        {
            return _source?.Register(disposable) ?? default;
        }

        bool IEquatable<LifetimeToken>.Equals(LifetimeToken other)
        {
            return Equals(other);
        }
    }
}
