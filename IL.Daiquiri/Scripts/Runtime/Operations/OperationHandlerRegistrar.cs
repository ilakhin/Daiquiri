using System.Collections.Generic;
using JetBrains.Annotations;

namespace IL.Daiquiri.Operations
{
    public sealed class OperationHandlerRegistrar : IModule
    {
        private readonly OperationManager _operationManager;
        private readonly IReadOnlyCollection<IOperationHandler> _operationHandlers;

        private LifetimeTokenSource _lifetimeTokenSource;

        [UsedImplicitly]
        public OperationHandlerRegistrar(OperationManager operationManager, IOperationHandler[] operationHandlers)
        {
            _operationManager = operationManager;
            _operationHandlers = operationHandlers;
        }

        private void AddHandlers(LifetimeToken lifetimeToken)
        {
            foreach (var handler in _operationHandlers)
            {
                _operationManager.AddHandler(handler, lifetimeToken);
            }
        }

        int IModule.Priority => 80;

        void IModule.Initialize()
        {
            _lifetimeTokenSource = new LifetimeTokenSource(_operationHandlers.Count);

            AddHandlers(_lifetimeTokenSource.Token);
        }

        void IModule.Deinitialize()
        {
            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;
        }
    }
}
