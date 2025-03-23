using System;
using System.Collections.Generic;
using IL.Daiquiri.Elements;
using IL.Daiquiri.Fragments;

namespace IL.Daiquiri.Popups
{
    public class PopupPayload : FragmentPayload
    {
        public PopupPayload(Type viewModelType) : base(viewModelType)
        {
        }

        public PopupPayload(Type viewModelType, string viewGuid) : base(viewModelType, viewGuid)
        {
        }

        public PopupPayload(Type viewModelType, string viewGuid, IReadOnlyDictionary<string, IElementPayload> childPayloads) : base(viewModelType, viewGuid, childPayloads)
        {
        }

        public bool IgnoreOutsideClicks
        {
            get;
            set;
        }
    }
}
