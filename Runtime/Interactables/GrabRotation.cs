using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.dgn.XR.Extensions
{
    public enum DirectionAxis { Forward, Backward, Left, Right, Up, Down}

    [AddComponentMenu("DGN/XR/Interactable/GrabRotation")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabRotation : MonoBehaviour
    {
        public float rotSpeed = 100f;

        [SerializeField]
        private DirectionAxis inputX_rotate = DirectionAxis.Backward;
        [SerializeField]
        private DirectionAxis inputY_rotate = DirectionAxis.Right;

        private XRGrabInteractable grabInteractable;
        private HandPresence handPresence;
        private XRBaseInteractor xrInteractor;
        private Transform attachTransform;
        private Quaternion mLocalRot_attachTransform;

        void Start()
        {
            grabInteractable = this.GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        void OnGrabbed(SelectEnterEventArgs args)
        {
            attachTransform = grabInteractable.attachTransform;
            if (attachTransform)
            {
                mLocalRot_attachTransform = attachTransform.localRotation;
            }
            xrInteractor = args.interactor;
            handPresence = args.interactor.attachTransform.GetComponentInChildren<HandPresence>();
        }

        void OnReleased(SelectExitEventArgs args)
        {
            if (attachTransform)
            {
                attachTransform.localRotation = mLocalRot_attachTransform;
            }
            attachTransform = null;
            xrInteractor = null;
            handPresence = null;
        }

        void Update()
        {
            if (handPresence && attachTransform && xrInteractor)
            {
                Vector2 input = handPresence.GetPrimary2DAxis();
                if (input != Vector2.zero)
                {
                    Rotate(inputX_rotate, input.x);
                    Rotate(inputY_rotate, input.y);
                }
            }
        }

        private void Rotate(DirectionAxis directionAxis, float val) {
            Vector3 dir = Vector3.zero;
            switch (directionAxis) {
                case DirectionAxis.Forward:
                    dir = xrInteractor.transform.forward;
                    break;
                case DirectionAxis.Backward:
                    dir = xrInteractor.transform.forward;
                    val = -val;
                    break;
                case DirectionAxis.Right:
                    dir = xrInteractor.transform.right;
                    break;
                case DirectionAxis.Left:
                    dir = xrInteractor.transform.right;
                    val = -val;
                    break;
                case DirectionAxis.Up:
                    dir = xrInteractor.transform.up;
                    break;
                case DirectionAxis.Down:
                    dir = xrInteractor.transform.up;
                    val = -val;
                    break;
            }
            attachTransform.RotateAround(xrInteractor.transform.position, dir, val * rotSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (attachTransform)
            {
                attachTransform.localRotation = mLocalRot_attachTransform;
            }
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}