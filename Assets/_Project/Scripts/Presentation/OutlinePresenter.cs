using System;
using System.Collections.Generic;
using UnityEngine;
using VRTraining.Interactions;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    public sealed class OutlinePresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private OutlineTarget[] targets;

        private readonly Dictionary<string, OutlineTarget> targetsById =
            new Dictionary<string, OutlineTarget>(StringComparer.Ordinal);

        private OutlineTarget activeTarget;

        private void Awake()
        {
            if (targets == null || targets.Length == 0)
                targets = FindObjectsByType<OutlineTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var target in targets)
            {
                if (target == null)
                    continue;

                var trainingTarget = target.GetComponent<TrainingTarget>();
                var id = trainingTarget != null ? trainingTarget.TargetId : null;
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                if (!targetsById.TryAdd(id, target))
                    Debug.LogError($"Duplicate outline target id: '{id}'.", target);
            }
        }

        private void OnEnable()
        {
            if (scenarioController != null)
                scenarioController.StateChanged += Refresh;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDisable()
        {
            if (scenarioController != null)
                scenarioController.StateChanged -= Refresh;

            SetActiveTarget(null);
        }

        private void Refresh()
        {
            var expectedAction = scenarioController != null
                ? scenarioController.CurrentExpectedAction
                : null;

            if (expectedAction == null ||
                !targetsById.TryGetValue(expectedAction.TargetId, out var target))
            {
                SetActiveTarget(null);
                return;
            }

            SetActiveTarget(target);
        }

        private void SetActiveTarget(OutlineTarget target)
        {
            if (activeTarget == target)
                return;

            if (activeTarget != null)
                activeTarget.SetHighlighted(false);

            activeTarget = target;

            if (activeTarget != null)
                activeTarget.SetHighlighted(true);
        }
    }
}
