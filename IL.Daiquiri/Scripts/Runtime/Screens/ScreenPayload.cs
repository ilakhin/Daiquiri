using System;
using System.Collections.Generic;
using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;

namespace IL.Daiquiri.Screens
{
    public class ScreenPayload : FragmentPayload
    {
        public ScreenPayload(Type viewModelType) : base(viewModelType)
        {
        }

        public ScreenPayload(Type viewModelType, string viewGuid) : base(viewModelType, viewGuid)
        {
        }

        public ScreenPayload(Type viewModelType, string viewGuid, IReadOnlyDictionary<string, IElementPayload> childPayloads) : base(viewModelType, viewGuid, childPayloads)
        {
        }
    }
}
