using System;
using System.Collections.Generic;
using IL.Daiquiri.Elements;

namespace IL.Daiquiri.Fragments
{
    public class FragmentPayload : ElementPayload
    {
        public FragmentPayload(Type viewModelType) : base(viewModelType)
        {
        }

        public FragmentPayload(Type viewModelType, string viewGuid) : base(viewModelType, viewGuid)
        {
        }

        public FragmentPayload(Type viewModelType, string viewGuid, IReadOnlyDictionary<string, IElementPayload> childPayloads) : base(viewModelType, viewGuid, childPayloads)
        {
        }
    }
}
