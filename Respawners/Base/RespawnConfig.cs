using UnityEngine;

namespace com.dgn.XR.Extensions
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RespawnConfig", menuName = "DGN/Respawn Behavior/RespawnConfig")]
    public class RespawnConfig : ScriptableObject
    {
        [SerializeField]
        public float respawnTime = 1.5f;

        [Tooltip("If this object collides with another object that has this layer, it will be respawned.")]
        [SerializeField]
        public LayerMask layermask = ~0;

        [Tooltip("If this object falls down below this value, it will be respawned.")]
        [SerializeField]
        public float fallZone = -50f;
    }
}