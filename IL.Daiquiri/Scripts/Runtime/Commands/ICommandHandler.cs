using System;

namespace IL.Daiquiri.Commands
{
    public interface ICommandHandler
    {
        bool IsValidUnitType(Type unitType);

        bool ExecuteUnit(ICommandUnit unit);
    }
}
