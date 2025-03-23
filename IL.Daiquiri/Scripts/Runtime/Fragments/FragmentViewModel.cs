using IL.Daiquiri.Elements;
using JetBrains.Annotations;

namespace IL.Daiquiri.Fragments
{
    public class FragmentViewModel : FragmentViewModel<FragmentPayload>
    {
        [UsedImplicitly]
        public FragmentViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }

    public class FragmentViewModel<TPayload> : ElementViewModel<TPayload>
        where TPayload : FragmentPayload
    {
        [UsedImplicitly]
        public FragmentViewModel(ElementViewModelFactory viewModelFactory) : base(viewModelFactory)
        {
        }
    }
}
