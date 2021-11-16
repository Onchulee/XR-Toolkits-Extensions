
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// When this object touches other object with curtain layer [respawnWhenCollide]
/// for respawn time, it will be respawned to any empty XRSocketInteractor.
/// Default spawn position is at start position of object.
/// It won't respawn as long as the object is grabbed.
/// </summary>
namespace com.dgn.XR.Extensions
{
    [AddComponentMenu("DGN/Respawn Behavior/XR Socket Respawner", 3)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
    public class XRSocketRespawner : RespawnBehavior
    {
        [SerializeField]
        public XRSocketInteractor[] respawnAt;

        private XRGrabInteractable grabInteractable;

        protected override void Awake()
        {
            base.Awake();
            grabInteractable = this.GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        protected override void Respawn()
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            Vector3 spawnPos = defaultPosition;
            Vector3 spawnAngles = defaultEulerAngles;

            foreach (XRSocketInteractor socket in respawnAt)
            {
                if (socket && !socket.selectTarget)
                {
                    spawnPos = socket.attachTransform.position + grabInteractable.attachTransform.localPosition;
                    spawnAngles = socket.attachTransform.eulerAngles + grabInteractable.attachTransform.localEulerAngles;
                    break;
                }
            }
            this.transform.position = spawnPos;
            this.transform.eulerAngles = spawnAngles;
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            SetEnableRespawn(false);
            respawnTimer = 0;
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            SetEnableRespawn(true);
            respawnTimer = 0;
        }
    }
}