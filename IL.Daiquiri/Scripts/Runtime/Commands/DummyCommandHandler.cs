using JetBrains.Annotations;

namespace IL.Daiquiri.Commands
{
    public sealed class DummyCommandHandler : CommandHandler<DummyCommandUnit>
    {
        [UsedImplicitly]
        public DummyCommandHandler()
        {
        }

        protected override bool ExecuteUnit(DummyCommandUnit unit)
        {
            return true;
        }
    }
}
