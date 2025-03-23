using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using UnityEngine.Pool;

namespace IL.Daiquiri.Commands
{
    public sealed class CommandManager : IModule
    {
        private List<ICommandHandler> _allHandlers;
        private Dictionary<Type, ICommandHandler> _concreteHandlers;

        [UsedImplicitly]
        public CommandManager()
        {
        }

        public void AddHandler(ICommandHandler handler, out IDisposable disposable)
        {
            _allHandlers.Add(handler);

            disposable = Disposable.Create((Manager: this, Handelr: handler), static stateTuple =>
            {
                var (manager, handler) = stateTuple;

                manager.RemoveHandler(handler);
            });
        }

        public void AddHandler(ICommandHandler handler, LifetimeToken lifetimeToken)
        {
            AddHandler(handler, out var disposable);

            lifetimeToken.Register(disposable);
        }

        public void RemoveHandler(ICommandHandler handler)
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

        public bool ExecuteUnit(ICommandUnit unit)
        {
            try
            {
                var unitType = unit.GetType();

                if (!TryGetHandler(unitType, out var handler))
                {
                    throw new NotSupportedException($"Invalid unit type \"{unitType.Name}\"!");
                }

                var result = handler.ExecuteUnit(unit);

                return result;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                return false;
            }
        }

        private bool TryGetHandler(Type unitType, out ICommandHandler handler)
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
            _allHandlers = new List<ICommandHandler>(32);
            _concreteHandlers = new Dictionary<Type, ICommandHandler>(32);
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
