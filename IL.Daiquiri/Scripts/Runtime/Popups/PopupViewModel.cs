using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace IL.Daiquiri.Popups
{
    public class PopupViewModel : PopupViewModel<PopupPayload>
    {
        [UsedImplicitly]
        public PopupViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }

    public class PopupViewModel<TPayload> : FragmentViewModel<TPayload>, IPopupViewModel
        where TPayload : PopupPayload
    {
        private Subject<IPopupViewModel> _closeSubject;

        [UsedImplicitly]
        public PopupViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }

        public void RequestClose()
        {
            _closeSubject?.OnNext(this);
        }

        public virtual void OnOutsideClick(Camera camera, Vector3 screenPoint)
        {
            _closeSubject?.OnNext(this);
        }

        void IPopupViewModel.SetCloseSubject(Subject<IPopupViewModel> closeSubject)
        {
            _closeSubject = closeSubject;
        }

        void IPopupViewModel.OnOutsideClick(Camera camera, Vector3 screenPoint)
        {
            if (Payload.IgnoreOutsideClicks)
            {
                return;
            }

            OnOutsideClick(camera, screenPoint);
        }
    }
}
