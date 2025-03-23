using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IL.Daiquiri.Elements
{
    [Serializable]
    public sealed class ElementBinderEntry
    {
        [Required]
        [SerializeField]
        private string _key;

        // TODO: Добавить валидацию
        [Required]
        [SerializeField]
        private MonoBehaviour _binder;

        public string Key => _key;

        public IElementBinder Binder => _binder.GetComponent<IElementBinder>();
    }
}
