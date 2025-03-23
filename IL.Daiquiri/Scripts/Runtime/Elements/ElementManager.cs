using System;
using JetBrains.Annotations;
using ObservableCollections;
using R3;

namespace IL.Daiquiri.Elements
{
    public sealed class ElementManager : IModule
    {
        private readonly ElementViewModelFactory _viewModelFactory;

        private ObservableList<IElementViewModel> _viewModels;

        [UsedImplicitly]
        public ElementManager(ElementViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public IReadOnlyObservableList<IElementViewModel> ViewModels => _viewModels;

        public IElementViewModel CreateElement(IElementPayload payload, out IDisposable disposable)
        {
            var viewModel = _viewModelFactory.Create(payload.ViewModelType);

            viewModel.Initialize(payload);

            _viewModels.Add(viewModel);

            disposable = Disposable.Create((Manager: this, ViewModel: viewModel), static stateTuple =>
            {
                var (manager, viewModel) = stateTuple;

                manager.DestroyElement(viewModel);
            });

            return viewModel;
        }

        public IElementViewModel CreateElement(IElementPayload payload, LifetimeToken lifetimeToken)
        {
            var viewModel = CreateElement(payload, out var disposable);

            lifetimeToken.Register(disposable);

            return viewModel;
        }

        public void DestroyElement(IElementViewModel viewModel)
        {
            var result = _viewModels.Remove(viewModel);

            if (!result)
            {
                return;
            }

            viewModel.Deinitialize();
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 10;

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
