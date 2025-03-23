using System;
using System.Collections.Generic;

namespace IL.Daiquiri
{
    public sealed class LifetimeTokenSource : IDisposable
    {
        private List<IDisposable> _disposables;

        public LifetimeTokenSource()
        {
            _disposables = new List<IDisposable>();
        }

        public LifetimeTokenSource(int capacity)
        {
            _disposables = capacity >= 0 ? new List<IDisposable>(capacity) : throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        public bool IsTerminationRequested
        {
            get;
            private set;
        }

        public LifetimeToken Token => new(this);

        internal LifetimeTokenRegistration Register(IDisposable disposable)
        {
            if (IsTerminationRequested)
            {
                disposable.Dispose();

                return default;
            }

            _disposables.Add(disposable);

            return new LifetimeTokenRegistration(_disposables, disposable);
        }

        public void Terminate()
        {
            if (IsTerminationRequested)
            {
                return;
            }

            IsTerminationRequested = true;

            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception exception)
                {
                    LifetimeTokenUtility.PublishUnobservedException(exception);
                }
            }

            _disposables.Clear();
            _disposables = null;
        }

        void IDisposable.Dispose()
        {
            Terminate();
        }
    }
}
