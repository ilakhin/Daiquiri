using System;

namespace IL.Daiquiri.Commands
{
    public abstract class CommandHandler<TUnit> : ICommandHandler
        where TUnit : class, ICommandUnit
    {
        protected virtual bool IsValidUnitType(Type unitType)
        {
            return typeof(TUnit).IsAssignableFrom(unitType);
        }

        protected abstract bool ExecuteUnit(TUnit unit);

        bool ICommandHandler.IsValidUnitType(Type unitType)
        {
            return IsValidUnitType(unitType);
        }

        bool ICommandHandler.ExecuteUnit(ICommandUnit unit)
        {
            return ExecuteUnit((TUnit)unit);
        }
    }
}
