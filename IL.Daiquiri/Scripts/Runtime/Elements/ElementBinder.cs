using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace IL.Daiquiri.Elements
{
    public class ElementBinder : ElementBinder<ElementViewModel, ElementPayload>
    {
    }

    public class ElementBinder<TViewModel> : ElementBinder<TViewModel, ElementPayload>
        where TViewModel : ElementViewModel<ElementPayload>
    {
    }

    public class ElementBinder<TViewModel, TPayload> : MonoBehaviour, IElementBinder
        where TViewModel : ElementViewModel<TPayload>
        where TPayload : ElementPayload
    {
        [SerializeField]
        private ElementBinderEntry[] _childEntries;

        private CancellationTokenSource _cancellationTokenSource;
        private LifetimeTokenSource _lifetimeTokenSource;
        private Dictionary<string, IElementBinder> _childBinders;

        protected CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationTokenUtility.CancelledToken;

        protected LifetimeToken LifetimeToken => _lifetimeTokenSource?.Token ?? LifetimeTokenUtility.TerminatedToken;

        protected TViewModel ViewModel
        {
            get;
            private set;
        }

        protected virtual void OnInitialize()
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, CancellationToken.None);
            _lifetimeTokenSource = new LifetimeTokenSource();
        }

        private void OnInitializeChildBinders(IReadOnlyDictionary<string, IElementViewModel> childViewModels)
        {
            var childEntries = _childEntries.ToDictionary(static entry => entry.Key, StringComparer.Ordinal);
            var childBinders = new Dictionary<string, IElementBinder>();

            foreach (var (key, childViewModel) in childViewModels)
            {
                var childEntry = childEntries[key];
                var childBinder = childEntry.Binder;

                childBinder.Initialize(childViewModel);
                childBinders.Add(key, childBinder);
            }

            _childBinders = childBinders;
        }

        protected virtual void OnDeinitialize()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            _lifetimeTokenSource.Terminate();
            _lifetimeTokenSource = null;

            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        private void OnDeinitializeChildBinders()
        {
            var childBinders = _childBinders;

            _childBinders = null;

            foreach (var childBinder in childBinders.Values)
            {
                childBinder.Deinitialize();
            }

            childBinders.Clear();
        }

        void IElementBinder.Initialize(IElementViewModel viewModel)
        {
            ViewModel = (TViewModel)viewModel;

            OnInitializeChildBinders(viewModel.ChildViewModels);
            OnInitialize();
        }

        void IElementBinder.Deinitialize()
        {
            ViewModel = null;

            OnDeinitializeChildBinders();
            OnDeinitialize();
        }
    }
}
