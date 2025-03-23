using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;
using JetBrains.Annotations;

namespace IL.Daiquiri.Screens
{
    public class ScreenViewModel : ScreenViewModel<ScreenPayload>
    {
        [UsedImplicitly]
        public ScreenViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }

    public class ScreenViewModel<TPayload> : FragmentViewModel<TPayload>
        where TPayload : ScreenPayload
    {
        [UsedImplicitly]
        public ScreenViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }
}
