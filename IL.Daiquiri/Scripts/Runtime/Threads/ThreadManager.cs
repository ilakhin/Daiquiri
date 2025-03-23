using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Core;
using IL.Daiquiri.Operations;
using JetBrains.Annotations;

namespace IL.Daiquiri.Threads
{
    public sealed class ThreadManager : IModule
    {
        private readonly OperationManager _operationManager;

        private Dictionary<string, Thread> _threads;

        [UsedImplicitly]
        public ThreadManager(OperationManager operationManager)
        {
            _operationManager = operationManager;
        }

        public UniTask EnqueueOperationUnitAsync(string threadId, IOperationUnit operationUnit, int operationPriority, CancellationToken cancellationToken)
        {
            var thread = GetThread(threadId);

            return thread.EnqueueOperationUnitAsync(operationUnit, operationPriority, cancellationToken);
        }

        private Thread GetThread(string threadId)
        {
            if (_threads.TryGetValue(threadId, out var thread))
            {
                return thread;
            }

            thread = new Thread(_operationManager);
            thread.Initialize();

            _threads[threadId] = thread;

            return thread;
        }

        int IModule.Priority => 10;

        void IModule.Initialize()
        {
            _threads = new Dictionary<string, Thread>(StringComparer.Ordinal);
        }

        void IModule.Deinitialize()
        {
            foreach (var thread in _threads.Values)
            {
                thread.Deinitialize();
            }

            _threads.Clear();
            _threads = null;
        }
    }
}
