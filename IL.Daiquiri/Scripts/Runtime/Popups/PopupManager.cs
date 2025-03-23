using System;
using IL.Daiquiri.Fragments;
using JetBrains.Annotations;
using ObservableCollections;
using R3;
using UnityEngine;

namespace IL.Daiquiri.Popups
{
    public sealed class PopupManager : IModule
    {
        private readonly FragmentManager _fragmentManager;

        private ObservableList<IPopupViewModel> _viewModels;
        private Subject<IPopupViewModel> _closeSubject;

        [UsedImplicitly]
        public PopupManager(FragmentManager fragmentManager)
        {
            _fragmentManager = fragmentManager;
        }

        public IReadOnlyObservableList<IPopupViewModel> ViewModels => _viewModels;

        public IPopupViewModel CreatePopup(PopupPayload payload, out IDisposable disposable)
        {
            HideLastPopup();

            var viewModel = (IPopupViewModel)_fragmentManager.CreateFragment(payload, out var fragmentDisposable);

            viewModel.SetCloseSubject(_closeSubject);

            _viewModels.Add(viewModel);

            var popupDisposable = Disposable.Create((Manager: this, ViewModel: viewModel), static stateTuple =>
            {
                var (manager, viewModel) = stateTuple;

                manager.DestroyPopup(viewModel, false);
            });

            disposable = Disposable.Combine(popupDisposable, fragmentDisposable);

            return viewModel;
        }

        public IPopupViewModel CreatePopup(PopupPayload payload, LifetimeToken lifetimeToken)
        {
            var viewModel = CreatePopup(payload, out var disposable);

            lifetimeToken.Register(disposable);

            return viewModel;
        }

        public void DestroyPopup(IPopupViewModel viewModel)
        {
            DestroyPopup(viewModel, true);
        }

        private void DestroyPopup(IPopupViewModel viewModel, bool complete)
        {
            var result = _viewModels.Remove(viewModel);

            if (!result)
            {
                return;
            }

            if (complete)
            {
                _fragmentManager.DestroyFragment(viewModel);
            }

            ShowLastPopup();
        }

        private void ShowLastPopup()
        {
            if (TryGetLastViewModel(out var viewModel))
            {
                viewModel.Visible.Value = true;
            }
        }

        private void HideLastPopup()
        {
            if (TryGetLastViewModel(out var viewModel))
            {
                viewModel.Visible.Value = false;
            }
        }

        public void OnOutsideClick(Camera camera, Vector3 screenPoint)
        {
            if (TryGetLastViewModel(out var viewModel))
            {
                viewModel.OnOutsideClick(camera, screenPoint);
            }
        }

        private bool TryGetLastViewModel(out IPopupViewModel viewModel)
        {
            if (_viewModels.Count == 0)
            {
                viewModel = null;

                return false;
            }

            viewModel = _viewModels[^1];

            return true;
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 30;

        void IModule.Initialize()
        {
            _viewModels = new ObservableList<IPopupViewModel>();

            _closeSubject = new Subject<IPopupViewModel>();
            _closeSubject.Subscribe(this, static (viewModel, manager) => manager.DestroyPopup(viewModel));
        }

        void IModule.Deinitialize()
        {
            _viewModels.Clear();
            _viewModels = null;

            _closeSubject.Dispose();
            _closeSubject = null;
        }
    }
}
