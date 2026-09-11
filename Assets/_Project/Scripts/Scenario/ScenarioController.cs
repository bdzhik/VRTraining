using System;
using System.Collections.Generic;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Scenario.Definitions;

namespace VRTraining.Scenario
{
    /// <summary>
    /// Координатор сценария: принимает действия и публикует изменения состояния.
    /// </summary>
    public sealed class ScenarioController : MonoBehaviour
    {
        [SerializeField] private ScenarioDefinition scenarioDefinition;
        [SerializeField] private TrainingActionBus actionBus;

        private ScenarioSession session;

        public event Action StateChanged;
        public event Action<ScenarioEvaluation> ActionEvaluated;
        public event Action ScenarioCompleted;

        public ScenarioDefinition Definition => scenarioDefinition;
        public bool IsReady => session != null;
        public bool IsCompleted => session != null && session.IsCompleted;
        public int CurrentGroupIndex => session?.CurrentGroupIndex ?? -1;
        public int CurrentStepIndex => session?.CurrentStepIndex ?? -1;
        public StepGroupDefinition CurrentGroup => session?.CurrentGroup;
        public StepDefinition CurrentStep => session?.CurrentStep;
        public ExpectedActionDefinition CurrentExpectedAction => session?.CurrentExpectedAction;

        private void Awake()
        {
            if (scenarioDefinition == null)
            {
                Debug.LogError("Scenario Definition is not assigned.", this);
                enabled = false;
                return;
            }

            if (actionBus == null)
            {
                Debug.LogError("Training Action Bus is not assigned.", this);
                enabled = false;
                return;
            }

            try
            {
                session = new ScenarioSession(scenarioDefinition);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Cannot initialize scenario: {exception.Message}", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (actionBus != null)
                actionBus.ActionRaised += HandleAction;
        }

        private void Start()
        {
            if (session != null)
                StateChanged?.Invoke();
        }

        private void OnDisable()
        {
            if (actionBus != null)
                actionBus.ActionRaised -= HandleAction;
        }

        public StepStatus GetStepStatus(int groupIndex, int stepIndex)
        {
            return session == null
                ? StepStatus.NotStarted
                : session.GetStatus(groupIndex, stepIndex);
        }

        public IReadOnlyList<ScenarioResultEntry> GetResults()
        {
            return session?.GetResults() ?? Array.Empty<ScenarioResultEntry>();
        }

        private void HandleAction(TrainingAction action)
        {
            if (session == null)
                return;

            var evaluation = session.Process(action);
            if (!evaluation.WasProcessed)
                return;

            ActionEvaluated?.Invoke(evaluation);
            StateChanged?.Invoke();

            if (evaluation.ScenarioCompleted)
                ScenarioCompleted?.Invoke();
        }
    }
}
