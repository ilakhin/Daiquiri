using IL.Daiquiri.Elements;
using R3;
using UnityEngine;

namespace IL.Daiquiri.Popups
{
    public interface IPopupViewModel : IElementViewModel
    {
        void SetCloseSubject(Subject<IPopupViewModel> requestCloseSubject);

        void OnOutsideClick(Camera camera, Vector3 screenPoint);
    }
}
