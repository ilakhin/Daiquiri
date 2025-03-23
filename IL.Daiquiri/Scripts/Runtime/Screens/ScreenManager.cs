using System;
using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;
using JetBrains.Annotations;
using ObservableCollections;
using R3;

namespace IL.Daiquiri.Screens
{
    public sealed class ScreenManager : IModule
    {
        private readonly FragmentManager _fragmentManager;

        private ObservableList<IElementViewModel> _viewModels;

        [UsedImplicitly]
        public ScreenManager(FragmentManager fragmentManager)
        {
            _fragmentManager = fragmentManager;
        }

        public IReadOnlyObservableList<IElementViewModel> ViewModels => _viewModels;

        public IElementViewModel CreateScreen(ScreenPayload payload, out IDisposable disposable)
        {
            var viewModel = _fragmentManager.CreateFragment(payload, out var fragmentDisposable);

            _viewModels.Add(viewModel);

            var screenDisposable = Disposable.Create((Manager: this, ViewModel: viewModel), static stateTuple =>
            {
                var (manager, viewModel) = stateTuple;

                manager.DestroyScreen(viewModel, false);
            });

            disposable = Disposable.Combine(screenDisposable, fragmentDisposable);

            return viewModel;
        }

        public IElementViewModel CreateScreen(ScreenPayload payload, LifetimeToken lifetimeToken)
        {
            var viewModel = CreateScreen(payload, out var disposable);

            lifetimeToken.Register(disposable);

            return viewModel;
        }

        public void DestroyScreen(IElementViewModel viewModel)
        {
            DestroyScreen(viewModel, true);
        }

        private void DestroyScreen(IElementViewModel viewModel, bool complete)
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
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 30;

        void IModule.Initialize()
        {
            _viewModels = new ObservableList<IElementViewModel>();
        }

        void IModule.Deinitialize()
        {
            _viewModels.Clear();
            _viewModels = null;
        }
    }
}
