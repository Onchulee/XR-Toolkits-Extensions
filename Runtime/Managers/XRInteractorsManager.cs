using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// XR Interactors Manager helps controlling some behavior of interactors from same controller to prevent overlapped interactions.
/// Features:
/// 1. Prevent select action acts individually.
/// [eg. Ray Interactor and Direct Interactor from same controller won't call Select if any of them already called.]
/// </summary>
namespace com.dgn.XR.Extensions
{
    [DisallowMultipleComponent]
    public class XRInteractorsManager : Singleton<XRInteractorsManager>
    {
        public enum Controller { Left, Right}

        [Serializable]
        class XRInteractorConfig
        {
            [SerializeField]
            public XRBaseInteractor interactor;
            private UnityAction<SelectEnterEventArgs> onSelectAction;
            private UnityAction<SelectExitEventArgs> onUnselectAction;
            private bool active = false;

            public XRInteractorConfig(XRBaseInteractor baseInteractor,
                UnityAction<SelectEnterEventArgs> selectAction,
                UnityAction<SelectExitEventArgs> unselectAction)
            {
                interactor = baseInteractor;
                onSelectAction = selectAction;
                onUnselectAction = unselectAction;
            }

            public void StartActions()
            {
                if (interactor && !active)
                {
                    if (onSelectAction != null) interactor?.selectEntered.AddListener(onSelectAction);
                    if (onUnselectAction != null) interactor?.selectExited.AddListener(onUnselectAction);
                    active = true;
                }
            }

            public void StopActions()
            {
                if (interactor && active)
                {
                    if (onSelectAction != null) interactor?.selectEntered.RemoveListener(onSelectAction);
                    if (onUnselectAction != null) interactor?.selectExited.RemoveListener(onUnselectAction);
                    active = false;
                }
            }

            public static implicit operator bool(XRInteractorConfig exists)
            {
                return exists != null;
            }
        }
        
        [SerializeField]
        private XRBaseInteractor[] leftInteractors;
        private XRInteractorConfig[] leftInteractorConfigs;

        [SerializeField]
        private XRBaseInteractor[] rightInteractors;
        private XRInteractorConfig[] rightInteractorConfigs;

        protected override void Awake()
        {
            base.Awake();
            leftInteractorConfigs = CreateConfigs(leftInteractors);
            rightInteractorConfigs = CreateConfigs(rightInteractors);
        }

        public XRBaseInteractor[] GetInteractors(Controller side) {
            if (side == Controller.Left) return leftInteractors;
            if (side == Controller.Right) return rightInteractors;
            return new XRBaseInteractor[0];
        }

        public bool HasInteractor(XRBaseInteractor r_Interactor) {
            foreach (XRBaseInteractor interactor in leftInteractors) {
                if (interactor.Equals(r_Interactor)) return true;
            }
            foreach (XRBaseInteractor interactor in rightInteractors)
            {
                if (interactor.Equals(r_Interactor)) return true;
            }
            return false;
        }

        private void OnEnable()
        {
            InitializeConfigs(leftInteractorConfigs);
            InitializeConfigs(rightInteractorConfigs);
        }

        private void OnDisable()
        {
            TerminateConfigs(leftInteractorConfigs);
            TerminateConfigs(rightInteractorConfigs);
        }

        private void OnDestroy()
        {
            TerminateConfigs(leftInteractorConfigs);
            TerminateConfigs(rightInteractorConfigs);
        }

        private XRInteractorConfig[] CreateConfigs(XRBaseInteractor[] interactors)
        {
            XRInteractorConfig[] targetConfigs = new XRInteractorConfig[interactors.Length];
            for (int i = 0; i < interactors.Length; i++)
            {
                if (interactors[i])
                {
                    XRBaseInteractor target = interactors[i];
                    targetConfigs[i] = new XRInteractorConfig(
                        target,
                        delegate { DisableInteractors(interactors, target); },
                        delegate { EnableInteractors(interactors); }
                        );
                }
            }
            return targetConfigs;
        }

        private void InitializeConfigs(XRInteractorConfig[] targetConfigs)
        {
            foreach (XRInteractorConfig config in targetConfigs)
            {
                config?.StartActions();
            }
        }

        private void TerminateConfigs(XRInteractorConfig[] targetConfigs)
        {
            foreach (XRInteractorConfig config in targetConfigs)
            {
                config?.StopActions();
            }
        }

        private void DisableInteractors(XRBaseInteractor[] interactors, XRBaseInteractor ignore = null)
        {
            foreach (XRBaseInteractor interactor in interactors)
            {
                if (interactor) {
                    bool enableInteractions = (interactor == ignore);
                    interactor.allowHover = enableInteractions;
                    interactor.allowSelect = enableInteractions;
                }
            }
        }

        private void EnableInteractors(XRBaseInteractor[] interactors)
        {
            foreach (XRBaseInteractor interactor in interactors)
            {
                if (interactor) {
                    interactor.allowHover = true;
                    interactor.allowSelect = true;

                }
            }
        }

    }
}