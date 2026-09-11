using TMPro;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    public sealed class TrainingHUDPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private TMP_Text groupTitleText;
        [SerializeField] private TMP_Text groupInstructionText;
        [SerializeField] private TMP_Text stepDescriptionText;
        [SerializeField] private TMP_Text progressText;

        [Header("Context Action Panels")]
        [SerializeField] private GameObject briefingConfirmationPanel;
        [SerializeField] private string briefingConfirmationTargetId = "ui.briefing-confirm";
        [SerializeField] private GameObject completionConfirmationPanel;
        [SerializeField] private string completionConfirmationTargetId = "ui.training-complete";

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
        }

        private void Refresh()
        {
            if (scenarioController == null || !scenarioController.IsReady)
            {
                SetContextPanelsActive(null);
                return;
            }

            if (scenarioController.IsCompleted)
            {
                SetText(groupTitleText, "Тренировка завершена");
                SetText(groupInstructionText, string.Empty);
                SetText(stepDescriptionText, string.Empty);
                SetText(progressText, string.Empty);
                SetContextPanelsActive(null);
                return;
            }

            SetText(groupTitleText, scenarioController.CurrentGroup.Title);
            SetText(groupInstructionText, scenarioController.CurrentGroup.Instruction);
            SetText(stepDescriptionText, scenarioController.CurrentStep.Description);
            SetText(
                progressText,
                $"Группа {scenarioController.CurrentGroupIndex + 1}/{scenarioController.Definition.Groups.Count} · " +
                $"Шаг {scenarioController.CurrentStepIndex + 1}/{scenarioController.CurrentGroup.Steps.Count}");

            var expectedAction = scenarioController.CurrentExpectedAction;
            var expectedUiTargetId = expectedAction != null &&
                                     expectedAction.ActionType == TrainingActionType.PressUIButton
                ? expectedAction.TargetId
                : null;

            SetContextPanelsActive(expectedUiTargetId);
        }

        private void SetContextPanelsActive(string expectedTargetId)
        {
            SetActive(
                briefingConfirmationPanel,
                expectedTargetId == briefingConfirmationTargetId);
            SetActive(
                completionConfirmationPanel,
                expectedTargetId == completionConfirmationTargetId);
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
                target.SetActive(active);
        }

        private static void SetText(TMP_Text target, string value)
        {
            if (target != null)
                target.text = value;
        }
    }
}
