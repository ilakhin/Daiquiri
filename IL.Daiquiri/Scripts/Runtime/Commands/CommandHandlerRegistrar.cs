using System.Collections.Generic;
using IL.Daiquiri.Core;
using JetBrains.Annotations;

namespace IL.Daiquiri.Commands
{
    public sealed class CommandHandlerRegistrar : IModule
    {
        private readonly CommandManager _commandManager;
        private readonly IReadOnlyCollection<ICommandHandler> _commandHandlers;

        private LifetimeTokenSource _lifetimeTokenSource;

        [UsedImplicitly]
        public CommandHandlerRegistrar(CommandManager commandManager, ICommandHandler[] commandHandlers)
        {
            _commandManager = commandManager;
            _commandHandlers = commandHandlers;
        }

        private void AddHandlers(LifetimeToken lifetimeToken)
        {
            foreach (var handler in _commandHandlers)
            {
                _commandManager.AddHandler(handler, lifetimeToken);
            }
        }

        int IModule.Priority => 80;

        void IModule.Initialize()
        {
            _lifetimeTokenSource = new LifetimeTokenSource(_commandHandlers.Count);

            AddHandlers(_lifetimeTokenSource.Token);
        }

        void IModule.Deinitialize()
        {
            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;
        }
    }
}
