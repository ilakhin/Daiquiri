using System.Threading;
using IL.Daiquiri.Core;
using IL.Daiquiri.Fragments;
using IL.Daiquiri.Popups;
using JetBrains.Annotations;
using ObservableCollections;
using R3;
using UnityEngine;

namespace IL.Daiquiri.Blackout
{
    public sealed class BlackoutManager : IModule
    {
        private readonly FragmentManager _fragmentManager;
        private readonly PopupManager _popupManager;

        private CancellationTokenSource _cancellationTokenSource;
        private LifetimeTokenSource _lifetimeTokenSource;
        private Subject<(Camera Camera, Vector3 ScreenPoint)> _clickSubject;

        private BlackoutViewModel _viewModel;

        [UsedImplicitly]
        public BlackoutManager(FragmentManager fragmentManager, PopupManager popupManager)
        {
            _fragmentManager = fragmentManager;
            _popupManager = popupManager;
        }

        private void TryCreateBlackout()
        {
            if (_viewModel != null)
            {
                return;
            }

            // TODO: Вынести в конфиг
            var payload = new FragmentPayload(typeof(BlackoutViewModel), "f7dbb6c80f2b342f28f208b502b3022f");
            var viewModel = (BlackoutViewModel)_fragmentManager.CreateFragment(payload, _lifetimeTokenSource.Token);

            viewModel.SetClickSubject(_clickSubject);

            _viewModel = viewModel;
        }

        private void TryDestroyBlackout()
        {
            if (_viewModel == null)
            {
                return;
            }

            var viewModel = _viewModel;

            _viewModel.SetClickSubject(null);
            _viewModel = null;

            _fragmentManager.DestroyFragment(viewModel);
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 40;

        void IModule.Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lifetimeTokenSource = new LifetimeTokenSource();

            _clickSubject = new Subject<(Camera Camera, Vector3 ScreenPoint)>();
            _clickSubject
                .Subscribe(_popupManager, static (clickData, popupManager) =>
                {
                    popupManager.OnOutsideClick(clickData.Camera, clickData.ScreenPoint);
                })
                .RegisterTo(_lifetimeTokenSource.Token);

            _popupManager.ViewModels
                .ObserveCountChanged(true, _cancellationTokenSource.Token)
                .Subscribe(this, static (count, blackoutManager) =>
                {
                    if (count == 0)
                    {
                        blackoutManager.TryDestroyBlackout();
                    }
                    else
                    {
                        blackoutManager.TryCreateBlackout();
                    }
                })
                .RegisterTo(_lifetimeTokenSource.Token);
        }

        void IModule.Deinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;

            _clickSubject.Dispose();
            _clickSubject = null;
        }
    }
}
