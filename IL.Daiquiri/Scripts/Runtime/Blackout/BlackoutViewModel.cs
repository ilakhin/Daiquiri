using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace IL.Daiquiri.Blackout
{
    public sealed class BlackoutViewModel : FragmentViewModel
    {
        private Subject<(Camera Camera, Vector3 ScreenPoint)> _clickSubject;

        [UsedImplicitly]
        public BlackoutViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }

        public void SetClickSubject(Subject<(Camera Camera, Vector3 ScreenPoint)> clickSubject)
        {
            _clickSubject = clickSubject;
        }

        public void OnPointerClick(Camera camera, Vector3 screenPoint)
        {
            _clickSubject?.OnNext((camera, screenPoint));
        }
    }
}
