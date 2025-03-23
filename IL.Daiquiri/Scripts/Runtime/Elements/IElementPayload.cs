using System;
using System.Collections.Generic;

namespace IL.Daiquiri.Elements
{
    public interface IElementPayload
    {
        Type ViewModelType
        {
            get;
        }

        string ViewGuid
        {
            get;
        }

        IReadOnlyDictionary<string, IElementPayload> ChildPayloads
        {
            get;
        }
    }
}
