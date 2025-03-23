using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace IL.Daiquiri.Operations
{
    public abstract class OperationHandler<TUnit> : IOperationHandler
        where TUnit : class, IOperationUnit
    {
        protected virtual bool IsValidUnitType(Type unitType)
        {
            return typeof(TUnit).IsAssignableFrom(unitType);
        }

        protected abstract UniTask ExecuteUnitAsync(TUnit unit, CancellationToken cancellationToken);

        bool IOperationHandler.IsValidUnitType(Type unitType)
        {
            return IsValidUnitType(unitType);
        }

        UniTask IOperationHandler.ExecuteUnitAsync(IOperationUnit unit, CancellationToken cancellationToken)
        {
            return ExecuteUnitAsync((TUnit)unit, cancellationToken);
        }
    }
}
