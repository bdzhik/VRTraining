using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VRTraining.Core;

namespace VRTraining.Interactions
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public sealed class SocketActionSource : TrainingActionSource, IXRSelectFilter
    {
        [SerializeField] private string requiredInsertedTargetId = "ppe.safety-card";

        private XRSocketInteractor socket;

        public bool canProcess => isActiveAndEnabled;

        private void Awake()
        {
            socket = GetComponent<XRSocketInteractor>();
        }

        private void OnEnable()
        {
            if (socket == null)
                socket = GetComponent<XRSocketInteractor>();

            socket.selectFilters.Add(this);
            socket.selectEntered.AddListener(HandleInserted);
        }

        private void OnDisable()
        {
            if (socket != null)
            {
                socket.selectFilters.Remove(this);
                socket.selectEntered.RemoveListener(HandleInserted);
            }
        }

        /// <summary>
        /// Не позволяет XR Socket Interactor выбрать и зафиксировать чужой предмет.
        /// Проверка выполняется до события selectEntered, поэтому неправильный
        /// объект физически не защёлкнется в сокете.
        /// </summary>
        public bool Process(
            IXRSelectInteractor interactor,
            IXRSelectInteractable interactable)
        {
            var interactableComponent = interactable as Component;
            var insertedTarget = interactableComponent != null
                ? interactableComponent.GetComponentInParent<TrainingTarget>()
                : null;

            return insertedTarget != null &&
                   string.Equals(
                       insertedTarget.TargetId,
                       requiredInsertedTargetId,
                       StringComparison.Ordinal);
        }

        private void HandleInserted(SelectEnterEventArgs args)
        {
            var insertedComponent = args.interactableObject as Component;
            var insertedTarget = insertedComponent != null
                ? insertedComponent.GetComponentInParent<TrainingTarget>()
                : null;

            // Незарегистрированные декорации не считаются действиями сценария.
            if (insertedTarget == null)
                return;

            // До этого события допускается только предмет, прошедший Process.
            Publish(TrainingActionType.InsertObject);
        }
    }
}
