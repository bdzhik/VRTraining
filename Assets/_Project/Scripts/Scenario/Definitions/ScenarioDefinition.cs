using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario.Definitions
{
    [CreateAssetMenu(
        fileName = "TrainingScenario",
        menuName = "VR Training/Scenario Definition")]
    public sealed class ScenarioDefinition : ScriptableObject
    {
        [SerializeField] private string scenarioId = "conveyor-maintenance";
        [SerializeField] private string title = "Подготовка конвейера к обслуживанию";
        [SerializeField] private List<StepGroupDefinition> groups =
            new List<StepGroupDefinition>();

        public string ScenarioId => scenarioId;
        public string Title => title;
        public IReadOnlyList<StepGroupDefinition> Groups => groups;

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(scenarioId))
            {
                error = "Scenario id is empty.";
                return false;
            }

            if (groups == null || groups.Count == 0)
            {
                error = "Scenario must contain at least one group.";
                return false;
            }

            var stepIds = new HashSet<string>();

            for (var groupIndex = 0; groupIndex < groups.Count; groupIndex++)
            {
                var group = groups[groupIndex];
                if (group == null)
                {
                    error = $"Group {groupIndex} is null.";
                    return false;
                }

                if (group.Steps == null || group.Steps.Count == 0)
                {
                    error = $"Group '{group.Title}' has no steps.";
                    return false;
                }

                for (var stepIndex = 0; stepIndex < group.Steps.Count; stepIndex++)
                {
                    var step = group.Steps[stepIndex];
                    if (step == null)
                    {
                        error = $"Step {stepIndex} in group '{group.Title}' is null.";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(step.Id) || !stepIds.Add(step.Id))
                    {
                        error = $"Step id '{step.Id}' is empty or duplicated.";
                        return false;
                    }

                    if (step.ExpectedActions == null || step.ExpectedActions.Count == 0)
                    {
                        error = $"Step '{step.Id}' has no expected actions.";
                        return false;
                    }

                    for (var actionIndex = 0; actionIndex < step.ExpectedActions.Count; actionIndex++)
                    {
                        var action = step.ExpectedActions[actionIndex];
                        if (action == null || string.IsNullOrWhiteSpace(action.TargetId))
                        {
                            error = $"Step '{step.Id}' contains an invalid action.";
                            return false;
                        }
                    }
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
