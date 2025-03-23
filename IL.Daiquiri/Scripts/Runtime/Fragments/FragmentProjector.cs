using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using IL.Daiquiri.Assets;
using IL.Daiquiri.Core;
using IL.Daiquiri.Elements;
using JetBrains.Annotations;
using ObservableCollections;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IL.Daiquiri.Fragments
{
    public sealed class FragmentProjector : IModule
    {
        private readonly AssetManager _assetManager;
        private readonly FragmentManager _fragmentManager;
        private readonly Transform _parentTransform;

        private CancellationTokenSource _cancellationTokenSource;
        private LifetimeTokenSource _lifetimeTokenSource;
        private Dictionary<IElementViewModel, IElementBinder> _binders;
        private Dictionary<IElementViewModel, IDisposable> _disposables;

        [UsedImplicitly]
        public FragmentProjector(AssetManager assetManager, FragmentManager fragmentManager, ParentTransformProvider parentTransformProvider)
        {
            _assetManager = assetManager;
            _fragmentManager = fragmentManager;
            _parentTransform = parentTransformProvider.Transform;
        }

        private void OnAddViewModel(IElementViewModel viewModel)
        {
            var disposable = viewModel.Visible
                .Subscribe((Projector: this, ViewModel: viewModel), static (visible, stateTuple) =>
                {
                    var (projector, viewModel) = stateTuple;

                    if (visible)
                    {
                        projector.CreateView(viewModel);
                    }
                    else
                    {
                        projector.DestroyView(viewModel);
                    }
                });

            _disposables.Add(viewModel, disposable);
        }

        private void OnRemoveViewModel(IElementViewModel viewModel)
        {
            DestroyView(viewModel);

            if (!_disposables.Remove(viewModel, out var disposable))
            {
                return;
            }

            disposable.Dispose();
        }

        private void CreateView(IElementViewModel viewModel)
        {
            CreateViewAsync(viewModel, _cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid CreateViewAsync(IElementViewModel viewModel, CancellationToken cancellationToken)
        {
            var viewPrefab = await _assetManager.LoadAssetAsync<GameObject>(viewModel.Payload.ViewGuid, _lifetimeTokenSource.Token, cancellationToken);
            var viewGameObject = Object.Instantiate(viewPrefab, _parentTransform);
            var binder = viewGameObject.GetComponent<IElementBinder>();

            binder.Initialize(viewModel);

            _binders.Add(viewModel, binder);
        }

        private void DestroyView(IElementViewModel viewModel)
        {
            if (!_binders.Remove(viewModel, out var binder))
            {
                return;
            }

            binder.Deinitialize();
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 30;

        void IModule.Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lifetimeTokenSource = new LifetimeTokenSource();
            _binders = new Dictionary<IElementViewModel, IElementBinder>(32);
            _disposables = new Dictionary<IElementViewModel, IDisposable>(32);

            foreach (var viewModel in _fragmentManager.ViewModels)
            {
                OnAddViewModel(viewModel);
            }

            _fragmentManager.ViewModels
                .ObserveAdd(_cancellationTokenSource.Token)
                .Subscribe(this, static (addEvent, projector) => projector.OnAddViewModel(addEvent.Value))
                .RegisterTo(_lifetimeTokenSource.Token);

            _fragmentManager.ViewModels
                .ObserveRemove(_cancellationTokenSource.Token)
                .Subscribe(this, static (removeEvent, projector) => projector.OnRemoveViewModel(removeEvent.Value))
                .RegisterTo(_lifetimeTokenSource.Token);
        }

        void IModule.Deinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;

            foreach (var binder in _binders.Values)
            {
                binder.Deinitialize();
            }

            _binders.Clear();
            _binders = null;

            foreach (var disposable in _disposables.Values)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
            _disposables = null;
        }
    }
}
