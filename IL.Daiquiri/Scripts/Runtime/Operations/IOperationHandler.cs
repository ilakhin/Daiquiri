using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IL.Daiquiri.Operations
{
    public interface IOperationHandler
    {
        bool IsValidUnitType(Type unitType);

        UniTask ExecuteUnitAsync(IOperationUnit unit, CancellationToken cancellationToken);
    }
}
