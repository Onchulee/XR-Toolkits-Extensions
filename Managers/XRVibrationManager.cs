using UnityEngine;
using UnityEngine.XR;

namespace com.dgn.XR.Extensions
{
    public class XRVibrationManager : MonoBehaviour
    {
        private static XRVibrationManager singleton;
        public static XRVibrationManager Instance { get { return singleton; } }

        InputDevice rightXRController;
        bool isRightHandSupportsImpulse;
        InputDevice leftXRController;
        bool isLeftHandSupportsImpulse;

        private void Awake()
        {
            if (XRVibrationManager.Instance && XRVibrationManager.Instance != this)
            {
                Destroy(this);
            }
            else
            {
                singleton = this;
            }
        }

        void Start()
        {
            isRightHandSupportsImpulse = GetHapticCapabilities(XRNode.RightHand, out rightXRController);
            isLeftHandSupportsImpulse = GetHapticCapabilities(XRNode.LeftHand, out leftXRController);
        }

        private bool GetHapticCapabilities(XRNode xrNode, out InputDevice controller)
        {
            controller = InputDevices.GetDeviceAtXRNode(xrNode);
            HapticCapabilities hapcap = new HapticCapabilities();
            if (controller.isValid)
            {
                controller.TryGetHapticCapabilities(out hapcap);

                Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC BUFFER: " + hapcap.supportsBuffer);
                Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC IMPULSE: " + hapcap.supportsImpulse);
                Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC CHANNELS: " + hapcap.numChannels);
                Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC BUFFER FREQUENCY HZ: " + hapcap.bufferFrequencyHz);
            }
            return hapcap.supportsImpulse;
        }

        public void TriggerVibration(float amplitude, float duration, XRNode xrNode)
        {
            switch (xrNode)
            {
                case XRNode.RightHand:
                    if (isRightHandSupportsImpulse)
                    {
                        rightXRController.SendHapticImpulse(0, amplitude, duration);
                    }
                    break;
                case XRNode.LeftHand:
                    if (isLeftHandSupportsImpulse)
                    {
                        leftXRController.SendHapticImpulse(0, amplitude, duration);
                    }
                    break;
            }
        }

        public void StopVibration(XRNode xrNode)
        {
            switch (xrNode)
            {
                case XRNode.RightHand:
                    if (isRightHandSupportsImpulse)
                    {
                        rightXRController.SendHapticImpulse(0, 0);
                    }
                    break;
                case XRNode.LeftHand:
                    if (isLeftHandSupportsImpulse)
                    {
                        leftXRController.SendHapticImpulse(0, 0);
                    }
                    break;
            }
        }
    }
}