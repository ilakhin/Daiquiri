using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Core;
using IL.Daiquiri.Operations;

namespace IL.Daiquiri.Threads
{
    public sealed class Thread
    {
        private readonly OperationManager _operationManager;

        private CancellationTokenSource _cancellationTokenSource;
        private List<ThreadItem> _items;
        private WrappedStruct<bool> _processing;

        public Thread(OperationManager operationManager)
        {
            _operationManager = operationManager;
        }

        public void Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _items = new List<ThreadItem>(16);
            _processing = new WrappedStruct<bool>();
        }

        public void Deinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _items.Clear();
            _items = null;

            _processing = null;
        }

        public UniTask EnqueueOperationUnitAsync(IOperationUnit operationUnit, int operationPriority, CancellationToken cancellationToken)
        {
            var completionSource = new UniTaskCompletionSource();
            var item = new ThreadItem(operationUnit, operationPriority, completionSource, cancellationToken);

            EnqueueItem(item);

            if (!_processing.Value)
            {
                ProcessAsync(_processing, _items, _cancellationTokenSource.Token).Forget();
            }

            return completionSource.Task;
        }

        private void EnqueueItem(ThreadItem item)
        {
            var index = 0;

            foreach (var currentItem in _items)
            {
                if (currentItem.OperationPriority > item.OperationPriority)
                {
                    break;
                }

                index++;
            }

            _items.Insert(index, item);
        }

        private async UniTaskVoid ProcessAsync(WrappedStruct<bool> processing, IList<ThreadItem> items, CancellationToken cancellationToken)
        {
            processing.Value = true;

            try
            {
                await UniTask.Yield(cancellationToken);

                while (items.Count > 0)
                {
                    var item = items[0];

                    items.RemoveAt(0);

                    using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(item.CancellationToken, cancellationToken);

                    await ProcessAsync(item, cancellationTokenSource.Token);
                }
            }
            finally
            {
                processing.Value = false;
            }
        }

        private async UniTask ProcessAsync(ThreadItem item, CancellationToken cancellationToken)
        {
            try
            {
                await _operationManager.ExecuteUnitAsync(item.OperationUnit, cancellationToken);

                item.Promise.TrySetResult();
            }
            catch (Exception exception)
            {
                item.Promise.TrySetException(exception);
            }
        }
    }
}
