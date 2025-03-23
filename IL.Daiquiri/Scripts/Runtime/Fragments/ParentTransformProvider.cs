using UnityEngine;

namespace IL.Daiquiri.Fragments
{
    public sealed class ParentTransformProvider
    {
        public readonly Transform Transform;

        public ParentTransformProvider(Transform transform)
        {
            Transform = transform;
        }
    }
}
