using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRTraining.Core;

namespace VRTraining.Interactions
{
    [RequireComponent(typeof(XRBaseInteractable))]
    public sealed class SelectActionSource : TrainingActionSource
    {
        private XRBaseInteractable interactable;

        private void Awake()
        {
            interactable = GetComponent<XRBaseInteractable>();
        }

        private void OnEnable()
        {
            if (interactable == null)
                interactable = GetComponent<XRBaseInteractable>();

            interactable.selectEntered.AddListener(HandleSelected);
        }

        private void OnDisable()
        {
            if (interactable != null)
                interactable.selectEntered.RemoveListener(HandleSelected);
        }

        private void HandleSelected(SelectEnterEventArgs args)
        {
            Publish(TrainingActionType.SelectObject);
        }
    }
}
