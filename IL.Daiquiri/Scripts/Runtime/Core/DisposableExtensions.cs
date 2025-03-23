using System;

namespace IL.Daiquiri.Core
{
    public static class DisposableExtensions
    {
        public static TDisposable RegisterTo<TDisposable>(this TDisposable disposable, LifetimeToken lifetimeToken)
            where TDisposable : class, IDisposable
        {
            lifetimeToken.Register(disposable);

            return disposable;
        }
    }
}
