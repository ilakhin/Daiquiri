using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using R3;

namespace IL.Daiquiri.Elements
{
    public class ElementViewModel : ElementViewModel<ElementPayload>
    {
        [UsedImplicitly]
        public ElementViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }

    public class ElementViewModel<TPayload> : IElementViewModel
        where TPayload : ElementPayload
    {
        private readonly ElementViewModelFactory _viewModelFactory;

        private CancellationTokenSource _cancellationTokenSource;
        private LifetimeTokenSource _lifetimeTokenSource;
        private Dictionary<string, IElementViewModel> _childViewModels;

        [UsedImplicitly]
        public ElementViewModel(ElementViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        protected CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationTokenUtility.CancelledToken;

        protected LifetimeToken LifetimeToken => _lifetimeTokenSource?.Token ?? LifetimeTokenUtility.TerminatedToken;

        protected TPayload Payload
        {
            get;
            private set;
        }

        protected ReactiveProperty<bool> Visible
        {
            get;
            private set;
        }

        protected IReadOnlyDictionary<string, IElementViewModel> ChildViewModels => _childViewModels;

        protected virtual void OnInitialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _lifetimeTokenSource = new LifetimeTokenSource();

            Visible = new ReactiveProperty<bool>(true);
        }

        private void OnInitializeChildViewModels(IReadOnlyDictionary<string, IElementPayload> childPayloads)
        {
            var childViewModels = new Dictionary<string, IElementViewModel>(StringComparer.Ordinal);

            foreach (var (key, childPayload) in childPayloads)
            {
                var childViewModel = _viewModelFactory.Create(childPayload.ViewModelType);

                childViewModel.Initialize(childPayload);
                childViewModels.Add(key, childViewModel);
            }

            _childViewModels = childViewModels;
        }

        protected virtual void OnDeinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;

            Visible.Dispose();
            Visible = null;
        }

        private void OnDeinitializeChildViewModels()
        {
            var childViewModels = _childViewModels;

            _childViewModels = null;

            foreach (var viewModel in childViewModels.Values)
            {
                viewModel.Deinitialize();
            }

            childViewModels.Clear();
        }

        IElementPayload IElementViewModel.Payload => Payload;

        ReactiveProperty<bool> IElementViewModel.Visible => Visible;

        IReadOnlyDictionary<string, IElementViewModel> IElementViewModel.ChildViewModels => ChildViewModels;

        void IElementViewModel.Initialize(IElementPayload payload)
        {
            Payload = (TPayload)payload;

            OnInitializeChildViewModels(payload.ChildPayloads);
            OnInitialize();
        }

        void IElementViewModel.Deinitialize()
        {
            Payload = null;

            OnDeinitializeChildViewModels();
            OnDeinitialize();
        }
    }
}
