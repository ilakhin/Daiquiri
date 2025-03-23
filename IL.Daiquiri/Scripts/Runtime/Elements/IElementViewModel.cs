using System.Collections.Generic;
using R3;

namespace IL.Daiquiri.Elements
{
    public interface IElementViewModel
    {
        IElementPayload Payload
        {
            get;
        }

        ReactiveProperty<bool> Visible
        {
            get;
        }

        IReadOnlyDictionary<string, IElementViewModel> ChildViewModels
        {
            get;
        }

        void Initialize(IElementPayload payload);

        void Deinitialize();
    }
}
