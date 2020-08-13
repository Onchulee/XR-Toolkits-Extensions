using UnityEngine;

namespace com.dgn.XR.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask layermask, int layer)
        {
            return (layermask.value & 1 << layer) == 1 << layer;
        }
    }
}