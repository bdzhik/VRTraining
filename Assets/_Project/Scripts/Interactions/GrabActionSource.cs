using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VRTraining.Core;

namespace VRTraining.Interactions
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public sealed class GrabActionSource : TrainingActionSource
    {
        private XRGrabInteractable interactable;

        private void Awake()
        {
            interactable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            if (interactable == null)
                interactable = GetComponent<XRGrabInteractable>();

            interactable.selectEntered.AddListener(HandleSelected);
        }

        private void OnDisable()
        {
            if (interactable != null)
                interactable.selectEntered.RemoveListener(HandleSelected);
        }

        private void HandleSelected(SelectEnterEventArgs args)
        {
            // Установка предмета в Socket является отдельным действием, а не повторным Grab.
            if (args.interactorObject is XRSocketInteractor)
                return;

            Publish(TrainingActionType.GrabObject);
        }
    }
}
