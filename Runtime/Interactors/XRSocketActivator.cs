using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.dgn.XR.Extensions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRSocketInteractor), typeof(Collider))]
    public class XRSocketActivator : MonoBehaviour
    {
        [Tooltip("If XR Interactable object has layer within mask, XR Socket will be enabled.")]
        public LayerMask layermask;

        private XRSocketInteractor interactor;
        private HashSet<XRBaseInteractable> interactables = new HashSet<XRBaseInteractable>();

        private void Awake()
        {
            interactor = this.GetComponent<XRSocketInteractor>();
        }

        private void Start()
        {
            UpdateInteractor();
        }

        private void Update()
        {
            UpdateInteractor();
        }

        private void OnDisable()
        {
            interactables.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!layermask.HasLayer(other.gameObject.layer)) return;
            Transform target = other.transform;
            XRBaseInteractable interactable = target.GetComponent<XRBaseInteractable>();
            while (interactable == null && target.parent) {
                target = target.parent;
                interactable = target.GetComponent<XRBaseInteractable>();
            }
            if (interactable && interactable.isSelected)
            {
                interactables.Add(interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!layermask.HasLayer(other.gameObject.layer)) return;
            Transform target = other.transform;
            XRBaseInteractable interactable = target.GetComponent<XRBaseInteractable>();
            while (interactable == null && target.parent)
            {
                target = target.parent;
                interactable = target.GetComponent<XRBaseInteractable>();
            }
            if (interactable)
            {
                interactables.Remove(interactable);
            }
        }

        private void UpdateInteractor() {
            bool enableInteractions = interactor.selectTarget || (interactables.Count > 0);
            interactor.allowHover = enableInteractions; 
            interactor.allowSelect = enableInteractions;
        }
    }
}