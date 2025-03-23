using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Operations;

namespace IL.Daiquiri.Threads
{
    public sealed class ThreadItem
    {
        public readonly IOperationUnit OperationUnit;
        public readonly int OperationPriority;
        public readonly IPromise Promise;
        public readonly CancellationToken CancellationToken;

        public ThreadItem(IOperationUnit operationUnit, int operationPriority, IPromise promise, CancellationToken cancellationToken)
        {
            OperationUnit = operationUnit;
            OperationPriority = operationPriority;
            Promise = promise;
            CancellationToken = cancellationToken;
        }
    }
}
