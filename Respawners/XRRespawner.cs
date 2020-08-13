
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// When this object touches other object with curtain layer [respawnWhenCollide]
/// for respawn time, it will be respawned to spawn position.
/// Default spawn position is at start position of object.
/// It won't respawn as long as the object is grabbed.
/// </summary>

namespace com.dgn.XR.Extensions
{
    [AddComponentMenu("DGN/Respawn Behavior/XR Respawner", 2)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
    public class XRRespawner : RespawnBehavior
    {
        [SerializeField]
        public Transform respawnAt;

        private XRGrabInteractable grabInteractable;

        protected override void Awake()
        {
            base.Awake();
            grabInteractable = this.GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.onSelectEnter.AddListener(OnGrabbed);
            grabInteractable.onSelectExit.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            grabInteractable.onSelectEnter.RemoveListener(OnGrabbed);
            grabInteractable.onSelectEnter.RemoveListener(OnReleased);
        }

        private void OnGrabbed(XRBaseInteractor rBaseInteractor)
        {
            SetEnableRespawn(false);
            respawnTimer = 0;
        }

        private void OnReleased(XRBaseInteractor rBaseInteractor)
        {
            SetEnableRespawn(true);
            respawnTimer = 0;
        }

        protected override void Respawn()
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            if (respawnAt)
            {
                this.transform.position = respawnAt.position;
                this.transform.eulerAngles = respawnAt.eulerAngles;
            }
            else
            {
                this.transform.position = defaultPosition;
                this.transform.eulerAngles = defaultEulerAngles;
            }
        }
    }
}