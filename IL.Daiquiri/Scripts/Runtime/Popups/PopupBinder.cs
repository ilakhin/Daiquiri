using IL.Daiquiri.Fragments;

namespace IL.Daiquiri.Popups
{
    public class PopupBinder : PopupBinder<PopupViewModel, PopupPayload>
    {
    }

    public class PopupBinder<TViewModel> : PopupBinder<TViewModel, PopupPayload>
        where TViewModel : PopupViewModel<PopupPayload>
    {
    }

    public class PopupBinder<TViewModel, TPayload> : FragmentBinder<TViewModel, TPayload>
        where TViewModel : PopupViewModel<TPayload>
        where TPayload : PopupPayload
    {
    }
}
