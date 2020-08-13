using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace com.dgn.XR.Extensions
{
    public class HandPresence : MonoBehaviour
    {
        public enum HandState { Undefined, BareHand, Fist, Pointing, ThumbUp }

        public bool showController = false;
        public bool showHand = false;
        public InputDeviceCharacteristics controllerCharacteristics;
        public List<GameObject> controllerPrefabs;
        public GameObject handModelPrefab;

        [SerializeField]
        [ReadOnly]
        [ModifiedPropertyAttribute]
        private HandState state;
        public HandState CurrentState { get { return state; } }

        private InputDevice targetDevice;
        private GameObject spawnedController;
        private GameObject spawnedHandModel;
        private Animator handAnimator;
        private bool callLog = false;


        void Start()
        {
            TryInitialize();
        }

        void Update()
        {
            if (!targetDevice.isValid)
            {
                TryInitialize();
            }
            else
            {
                if (showController != spawnedController.activeInHierarchy)
                {
                    spawnedController.SetActive(showController);
                }
                if (showHand != spawnedHandModel.activeInHierarchy)
                {
                    spawnedHandModel.SetActive(showHand);
                }
                UpdateHandAnimation();
                UpdateHandState();
            }
        }

        private void TryInitialize()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

            foreach (InputDevice item in devices)
            {
                Log(item.name + item.characteristics);
            }

            if (devices.Count > 0)
            {
                targetDevice = devices[0];
                GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
                if (prefab)
                {
                    spawnedController = Instantiate(prefab, transform);
                }
                else
                {
                    LogError("Did not find corresponding controller model");
                    spawnedController = Instantiate(controllerPrefabs[0], transform);
                }

                spawnedHandModel = Instantiate(handModelPrefab, transform);
                handAnimator = spawnedHandModel.GetComponent<Animator>();

                spawnedController.SetActive(showController);
                spawnedHandModel.SetActive(showHand);
            }
        }

        private void UpdateHandAnimation()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                handAnimator.SetFloat("Trigger", triggerValue);
            }
            else
            {
                handAnimator.SetFloat("Trigger", 0);
            }

            if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                handAnimator.SetFloat("Grip", gripValue);
            }
            else
            {
                handAnimator.SetFloat("Grip", 0);
            }

            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool thumbTouch))
            {
                handAnimator.SetFloat("Thumb", thumbTouch ? 1f : 0);
            }
            else
            {
                handAnimator.SetFloat("Thumb", 0);
            }
        }

        private void UpdateHandState()
        {

            state = HandState.Undefined;

            if (GetTriggerValue() <= 0 && IsGripButtonPressed())
            {
                state = HandState.Pointing;
            }
            if (!IsNearTouchPrimaryThumbButtons() && IsGripButtonPressed() && IsTriggerButtonPressed())
            {
                state = HandState.ThumbUp;
            }
            if (IsNearTouchPrimaryThumbButtons() && IsGripButtonPressed() && IsTriggerButtonPressed())
            {
                state = HandState.Fist;
            }
            if (!IsNearTouchPrimaryThumbButtons() && !IsGripButtonPressed() && !IsTriggerButtonPressed())
            {
                state = HandState.BareHand;
            }
        }

        public bool IsMenuButtonPressed()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonValue) && menuButtonValue)
            {
                Log("Press Menu Button");
                return true;
            }
            return false;
        }

        public bool IsPrimaryButtonPressed()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
            {
                Log("Press Primary Button");
                return true;
            }
            return false;
        }

        public bool IsSecondaryButtonPressed()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue) && secondaryButtonValue)
            {
                Log("Press Secondary Button");
                return true;
            }
            return false;
        }

        public bool IsNearTouchPrimaryThumbButtons()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchValue) && primaryTouchValue)
            {
                Log("Press Primary Touch");
                return true;
            }
            return false;
        }

        public float GetTriggerValue()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
            {
                Log("Press Trigger value : " + triggerValue);
                return triggerValue;
            }
            return 0;
        }

        public bool IsTriggerButtonPressed()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonValue) && triggerButtonValue)
            {
                Log("Press Trigger Button");
                return true;
            }
            return false;
        }

        public float GetGripValue()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue > 0.1f)
            {
                Log("Press Grip value : " + gripValue);
                return gripValue;
            }
            return 0;
        }

        public bool IsGripButtonPressed()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButtonValue) && gripButtonValue)
            {
                Log("Press Grip Button");
                return true;
            }
            return false;
        }

        public Vector2 GetPrimary2DAxis()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue)
                && primary2DAxisValue != Vector2.zero)
            {
                Log("Primary Touchpad : " + primary2DAxisValue);
                return primary2DAxisValue;
            }
            return Vector2.zero;
        }

        private void Log(string msg)
        {
            if (callLog) Debug.Log("[" + name + "] " + msg);
        }
        private void LogError(string msg)
        {
            if (callLog) Debug.LogError("[" + name + "] " + msg);
        }
    }
}