using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace IL.Daiquiri.Operations
{
    public sealed class DummyOperationHandler : OperationHandler<DummyOperationUnit>
    {
        [UsedImplicitly]
        public DummyOperationHandler()
        {
        }

        protected override UniTask ExecuteUnitAsync(DummyOperationUnit unit, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
