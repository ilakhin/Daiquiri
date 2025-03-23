using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine.Pool;

namespace IL.Daiquiri.Operations
{
    public sealed class OperationManager : IModule
    {
        private List<IOperationHandler> _allHandlers;
        private Dictionary<Type, IOperationHandler> _concreteHandlers;

        [UsedImplicitly]
        public OperationManager()
        {
        }

        public void AddHandler(IOperationHandler handler, out IDisposable disposable)
        {
            _allHandlers.Add(handler);

            disposable = Disposable.Create((Manager: this, Handelr: handler), static stateTuple =>
            {
                var (manager, handler) = stateTuple;

                manager.RemoveHandler(handler);
            });
        }

        public void AddHandler(IOperationHandler handler, LifetimeToken lifetimeToken)
        {
            AddHandler(handler, out var disposable);

            lifetimeToken.Register(disposable);
        }

        public void RemoveHandler(IOperationHandler handler)
        {
            if (!_allHandlers.Remove(handler))
            {
                return;
            }

            using (ListPool<Type>.Get(out var removeTypes))
            {
                foreach (var (type, currentHandler) in _concreteHandlers)
                {
                    if (currentHandler != handler)
                    {
                        continue;
                    }

                    removeTypes.Add(type);
                }

                foreach (var removeType in removeTypes)
                {
                    _concreteHandlers.Remove(removeType);
                }
            }
        }

        public async UniTask ExecuteUnitAsync(IOperationUnit unit, CancellationToken cancellationToken)
        {
            var unitType = unit.GetType();

            if (!TryGetHandler(unitType, out var handler))
            {
                throw new NotSupportedException($"Invalid unit type \"{unitType.Name}\"!");
            }

            await handler.ExecuteUnitAsync(unit, cancellationToken);
        }

        private bool TryGetHandler(Type unitType, out IOperationHandler handler)
        {
            if (_concreteHandlers.TryGetValue(unitType, out handler))
            {
                return true;
            }

            foreach (var currentHandler in _allHandlers)
            {
                if (!currentHandler.IsValidUnitType(unitType))
                {
                    continue;
                }

                _concreteHandlers[unitType] = handler = currentHandler;

                return true;
            }

            return false;
        }

        int IModule.Priority => 10;

        void IModule.Initialize()
        {
            _allHandlers = new List<IOperationHandler>(32);
            _concreteHandlers = new Dictionary<Type, IOperationHandler>(32);
        }

        void IModule.Deinitialize()
        {
            _allHandlers.Clear();
            _allHandlers = null;

            _concreteHandlers.Clear();
            _concreteHandlers = null;
        }
    }
}
