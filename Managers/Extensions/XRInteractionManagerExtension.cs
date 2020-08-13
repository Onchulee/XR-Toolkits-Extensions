using System.Reflection;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public static class XRInteractionManagerExtension 
    {
        public static void ForceRelease(this XRInteractionManager manager, XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            MethodInfo manager_SelectExit = manager.GetType().GetMethod("SelectExit", BindingFlags.NonPublic | BindingFlags.Instance);
            manager_SelectExit?.Invoke(manager, new object[] { interactor, interactable });
        }
    }
}