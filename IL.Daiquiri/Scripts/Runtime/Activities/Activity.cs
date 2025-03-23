using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Core;
using JetBrains.Annotations;

namespace IL.Daiquiri.Activities
{
    public abstract class Activity : Activity<Payload>
    {
        [UsedImplicitly]
        protected Activity()
        {
        }
    }

    public abstract class Activity<TPayload> : IActivity
        where TPayload : Payload
    {
        private CancellationTokenSource _cancellationTokenSource;
        private LifetimeTokenSource _lifetimeTokenSource;

        [UsedImplicitly]
        protected Activity()
        {
        }

        protected TPayload Payload
        {
            get;
            private set;
        }

        protected virtual void OnInitialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lifetimeTokenSource = new LifetimeTokenSource();
        }

        protected virtual void OnDeinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;
        }

        protected abstract UniTask<Transition> OnExecuteAsync(LifetimeToken lifetimeToken, CancellationToken cancellationToken);

        void IActivity.Initialize(Payload payload)
        {
            Payload = (TPayload)payload;

            OnInitialize();
        }

        void IActivity.Deinitialize()
        {
            Payload = null;

            OnDeinitialize();
        }

        async UniTask<Transition> IActivity.ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            var transition = await OnExecuteAsync(_lifetimeTokenSource.Token, cancellationTokenSource.Token);

            return transition;
        }
    }
}
