using System;
using IL.Daiquiri.Elements;
using JetBrains.Annotations;
using ObservableCollections;
using R3;

namespace IL.Daiquiri.Fragments
{
    public sealed class FragmentManager : IModule
    {
        private readonly ElementManager _elementManager;

        private ObservableList<IElementViewModel> _viewModels;

        [UsedImplicitly]
        public FragmentManager(ElementManager elementManager)
        {
            _elementManager = elementManager;
        }

        public IReadOnlyObservableList<IElementViewModel> ViewModels => _viewModels;

        public IElementViewModel CreateFragment(FragmentPayload payload, out IDisposable disposable)
        {
            var viewModel = _elementManager.CreateElement(payload, out var elementDisposable);

            _viewModels.Add(viewModel);

            var fragmentDisposable = Disposable.Create((Manager: this, ViewModel: viewModel), static stateTuple =>
            {
                var (manager, viewModel) = stateTuple;

                manager.DestroyFragment(viewModel, false);
            });

            disposable = Disposable.Combine(fragmentDisposable, elementDisposable);

            return viewModel;
        }

        public IElementViewModel CreateFragment(FragmentPayload payload, LifetimeToken lifetimeToken)
        {
            var viewModel = CreateFragment(payload, out var disposable);

            lifetimeToken.Register(disposable);

            return viewModel;
        }

        public void DestroyFragment(IElementViewModel viewModel)
        {
            DestroyFragment(viewModel, true);
        }

        private void DestroyFragment(IElementViewModel viewModel, bool complete)
        {
            var result = _viewModels.Remove(viewModel);

            if (!result)
            {
                return;
            }

            if (complete)
            {
                _elementManager.DestroyElement(viewModel);
            }
        }

        // TODO: Вынести в конфиг
        int IModule.Priority => 20;

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
