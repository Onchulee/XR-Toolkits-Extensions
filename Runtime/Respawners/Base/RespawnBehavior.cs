using com.dgn.UnityAttributes;
using UnityEngine;

namespace com.dgn.XR.Extensions
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class RespawnBehavior : MonoBehaviour
    {
        protected Rigidbody rb;
        protected float respawnTimer;

        protected Vector3 defaultPosition;
        protected Vector3 defaultEulerAngles;

        [SerializeField]
        protected RespawnConfig config;

        [SerializeField]
        [OnEditorModeInspector]
        [Tooltip("True = Always check respawn")]
        protected bool burstMode;

        [SerializeField]
        [OnRuntimeInspector]
        [ReadOnly]
        [ModifiedPropertyAttribute]
        private bool enableRespawn = true;
        public bool EnableRespawn { get { return enableRespawn; } }

        private int count = 0;

        protected virtual void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            if (config == null)
            {
                Debug.LogWarning("Respawn config is not assigned.");
            }
        }

        protected virtual void Start()
        {
            defaultPosition = this.transform.position;
            defaultEulerAngles = this.transform.eulerAngles;
            enableRespawn = true;
        }

        protected void LateUpdate()
        {
            if (config && enableRespawn)
            {
                // case fall from floor
                if (transform.position.y < config.fallZone)
                {
                    count = 0;
                    respawnTimer = 0;
                    Respawn();
                }

                // case rb stop move
                if (count > 0 && rb.IsSleeping())
                {
                    count = 0;
                    respawnTimer = config.respawnTime;
                }
                // case respawn timer over
                if (respawnTimer > 0)
                {
                    respawnTimer -= Time.deltaTime;
                    if (respawnTimer <= 0)
                    {
                        Respawn();
                    }
                }
            }
        }

        protected void FixedUpdate()
        {
            // According to Unity: Order of Execution
            // Fixed Update will call before OnCollisionXXX
            // so we will reset to recount collsion in burst mode
            if (burstMode == true) count = 0;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (burstMode == false && IsInLayerMask(collision.gameObject.layer)) {
                count = count+1;
                if (respawnTimer <= 0) {
                    respawnTimer = config.respawnTime > 0? config.respawnTime : 1f;
                }
            }
        }

        protected virtual void OnCollisionStay(Collision collision) {
            if (burstMode == true && IsInLayerMask(collision.gameObject.layer))
            {
                count = count + 1;
                if (respawnTimer <= 0)
                {
                    respawnTimer = config.respawnTime > 0 ? config.respawnTime : 1f;
                }
            }
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if (burstMode == false && IsInLayerMask(collision.gameObject.layer)) {
                count = Mathf.Max(count-1, 0);
                if (count <= 0) {
                    respawnTimer = 0;
                }
            }
        }

        protected bool IsInLayerMask(int layer) {
            return config && enableRespawn && config.layermask.HasLayer(layer);
        }
        
        public void SetEnableRespawn(bool value)
        {
            enableRespawn = value;
        }

        protected abstract void Respawn();
    }
}