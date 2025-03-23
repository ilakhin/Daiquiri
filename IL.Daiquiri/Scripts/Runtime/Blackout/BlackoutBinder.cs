using IL.Daiquiri.Fragments;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IL.Daiquiri.Blackout
{
    [DisallowMultipleComponent]
    public sealed class BlackoutBinder : FragmentBinder<BlackoutViewModel>, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ViewModel?.OnPointerClick(eventData.pressEventCamera, eventData.position);
        }
    }
}
