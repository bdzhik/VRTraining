using System.Text;
using TMPro;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.Presentation
{
    public sealed class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioController scenarioController;
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private TMP_Text resultsText;

        private void OnEnable()
        {
            if (scenarioController != null)
                scenarioController.ScenarioCompleted += ShowResults;
        }

        private void Start()
        {
            if (resultsPanel != null)
                resultsPanel.SetActive(false);
        }

        private void OnDisable()
        {
            if (scenarioController != null)
                scenarioController.ScenarioCompleted -= ShowResults;
        }

        private void ShowResults()
        {
            if (resultsPanel != null)
                resultsPanel.SetActive(true);

            if (resultsText == null || scenarioController == null)
                return;

            var builder = new StringBuilder();
            string currentGroup = null;

            foreach (var entry in scenarioController.GetResults())
            {
                if (entry.GroupTitle != currentGroup)
                {
                    currentGroup = entry.GroupTitle;
                    if (builder.Length > 0)
                        builder.AppendLine();

                    builder.AppendLine(currentGroup);
                }

                builder.Append(GetStatusMark(entry.Status));
                builder.Append(' ');
                builder.Append(entry.StepDescription);
                builder.Append(" — ");
                builder.AppendLine(GetStatusText(entry.Status));
            }

            resultsText.text = builder.ToString();
        }

        private static string GetStatusMark(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Completed:
                    return "✓";
                case StepStatus.CompletedWithError:
                    return "!";
                case StepStatus.Skipped:
                    return "—";
                default:
                    return "·";
            }
        }

        private static string GetStatusText(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Completed:
                    return "выполнено";
                case StepStatus.CompletedWithError:
                    return "выполнено с ошибкой";
                case StepStatus.Skipped:
                    return "пропущено";
                default:
                    return "не выполнено";
            }
        }
    }
}
