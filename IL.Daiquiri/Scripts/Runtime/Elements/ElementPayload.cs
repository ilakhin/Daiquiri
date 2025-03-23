using System;
using System.Collections.Generic;

namespace IL.Daiquiri.Elements
{
    public class ElementPayload : IElementPayload
    {
        private readonly Type _viewModelType;
        private readonly string _viewGuid;
        private readonly IReadOnlyDictionary<string, IElementPayload> _childPayloads;

        public ElementPayload(Type viewModelType) : this(viewModelType, null, new Dictionary<string, IElementPayload>())
        {
        }

        public ElementPayload(Type viewModelType, string viewGuid) : this(viewModelType, viewGuid, new Dictionary<string, IElementPayload>())
        {
        }

        public ElementPayload(Type viewModelType, string viewGuid, IReadOnlyDictionary<string, IElementPayload> childPayloads)
        {
            _viewModelType = viewModelType;
            _viewGuid = viewGuid;
            _childPayloads = childPayloads;
        }

        Type IElementPayload.ViewModelType => _viewModelType;

        string IElementPayload.ViewGuid => _viewGuid;

        IReadOnlyDictionary<string, IElementPayload> IElementPayload.ChildPayloads => _childPayloads;
    }
}
