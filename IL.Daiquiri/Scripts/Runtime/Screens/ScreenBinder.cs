using IL.Daiquiri.Fragments;

namespace IL.Daiquiri.Screens
{
    public class ScreenBinder : ScreenBinder<ScreenViewModel, ScreenPayload>
    {
    }

    public class ScreenBinder<TViewModel> : ScreenBinder<TViewModel, ScreenPayload>
        where TViewModel : ScreenViewModel<ScreenPayload>
    {
    }

    public class ScreenBinder<TViewModel, TPayload> : FragmentBinder<TViewModel, TPayload>
        where TViewModel : ScreenViewModel<TPayload>
        where TPayload : ScreenPayload
    {
    }
}
