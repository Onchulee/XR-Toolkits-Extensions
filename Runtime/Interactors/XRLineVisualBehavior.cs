using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace com.dgn.XR.Extensions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRInteractorLineVisual))]
    public class XRLineVisualBehavior : MonoBehaviour
    {
        [SerializeField]
        private XRInteractorsManager.Controller controller;
        private XRBaseInteractor[] xrInteractors;
        private XRInteractorLineVisual xrLineVisual;
        private bool isInitialized;

        private void Awake()
        {
            xrLineVisual = this.GetComponent<XRInteractorLineVisual>();
        }

        private IEnumerator Start()
        {
            WaitUntil waitUntilFoundManager = new WaitUntil(()=>{ return XRInteractorsManager.Instance; });
            yield return waitUntilFoundManager;
            xrInteractors = XRInteractorsManager.Instance.GetInteractors(controller);
            OnInitialized();
        } 

        private void OnInitialized()
        { 
            if (xrInteractors==null) return;
            if(isInitialized) return;
            foreach (XRBaseInteractor interactor in xrInteractors)
            {
                if (interactor)
                {
                    interactor.selectEntered.AddListener(OnGrab);
                    interactor.selectExited.AddListener(OnRelease);
                }
            }
            isInitialized = true;
        }

        private void OnTerminated() {
            if (xrInteractors == null) return;
            if (!isInitialized) return;
            foreach (XRBaseInteractor interactor in xrInteractors)
            {
                if (interactor)
                {
                    interactor.selectEntered.RemoveListener(OnGrab);
                    interactor.selectExited.RemoveListener(OnRelease);
                }
            }
            isInitialized = false;
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (xrLineVisual) xrLineVisual.enabled = false;
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            if (xrLineVisual) xrLineVisual.enabled = true;
        }

        private void OnEnable()
        {
            OnInitialized();
        }

        private void OnDisable()
        {
            OnTerminated();
        }
    }
}