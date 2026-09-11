using System;
using System.Collections.Generic;
using VRTraining.Scenario.Definitions;

namespace VRTraining.Core
{
    /// <summary>
    /// Чистая C#-машина состояний. Не зависит от GameObject, XR, UI и звука.
    /// </summary>
    public sealed class ScenarioSession
    {
        private readonly ScenarioDefinition definition;
        private StepStatus[][] statuses;

        private int currentGroupIndex;
        private int currentStepIndex;
        private int currentActionIndex;

        public ScenarioSession(ScenarioDefinition definition)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

            if (!definition.TryValidate(out var error))
                throw new ArgumentException(error, nameof(definition));

            Reset();
        }

        public int CurrentGroupIndex => currentGroupIndex;
        public int CurrentStepIndex => currentStepIndex;
        public bool IsCompleted { get; private set; }

        public StepGroupDefinition CurrentGroup =>
            IsCompleted ? null : definition.Groups[currentGroupIndex];

        public StepDefinition CurrentStep =>
            IsCompleted ? null : CurrentGroup.Steps[currentStepIndex];

        public ExpectedActionDefinition CurrentExpectedAction =>
            IsCompleted ? null : CurrentStep.ExpectedActions[currentActionIndex];

        private void Reset()
        {
            statuses = new StepStatus[definition.Groups.Count][];

            for (var groupIndex = 0; groupIndex < definition.Groups.Count; groupIndex++)
            {
                statuses[groupIndex] =
                    new StepStatus[definition.Groups[groupIndex].Steps.Count];
            }

            currentGroupIndex = 0;
            currentStepIndex = 0;
            currentActionIndex = 0;
            IsCompleted = false;
        }

        public ScenarioEvaluation Process(TrainingAction action)
        {
            if (IsCompleted)
                return ScenarioEvaluation.Ignored(action, true);

            if (CurrentExpectedAction.Matches(action))
                return ProcessCorrectAction(action);

            var futureStepIndex = FindFutureStep(action);
            if (futureStepIndex >= 0)
                return ProcessSequenceViolation(action, futureStepIndex);

            // Действия другого типа не относятся к текущему шагу. Например,
            // повторный вход в зону не должен проваливать ожидаемый Grab-шаг.
            if (action.Type != CurrentExpectedAction.ActionType)
                return ScenarioEvaluation.Ignored(action, false);

            // Тип действия правильный, но цель неправильная:
            // вошли не в ту зону, взяли не тот объект или нажали не ту кнопку.
            return ProcessIncorrectAction(action);
        }

        public StepStatus GetStatus(int groupIndex, int stepIndex)
        {
            return statuses[groupIndex][stepIndex];
        }

        public IReadOnlyList<ScenarioResultEntry> GetResults()
        {
            var results = new List<ScenarioResultEntry>();

            for (var groupIndex = 0; groupIndex < definition.Groups.Count; groupIndex++)
            {
                var group = definition.Groups[groupIndex];
                for (var stepIndex = 0; stepIndex < group.Steps.Count; stepIndex++)
                {
                    results.Add(new ScenarioResultEntry(
                        groupIndex,
                        group.Title,
                        group.Steps[stepIndex].Description,
                        statuses[groupIndex][stepIndex]));
                }
            }

            return results;
        }

        private ScenarioEvaluation ProcessCorrectAction(TrainingAction action)
        {
            currentActionIndex++;

            if (currentActionIndex < CurrentStep.ExpectedActions.Count)
            {
                return new ScenarioEvaluation(
                    action, true, ScenarioFeedbackType.Correct, false);
            }

            statuses[currentGroupIndex][currentStepIndex] = StepStatus.Completed;
            AdvanceAfterStep();

            return new ScenarioEvaluation(
                action,
                true,
                ScenarioFeedbackType.Correct,
                IsCompleted);
        }

        private ScenarioEvaluation ProcessIncorrectAction(TrainingAction action)
        {
            statuses[currentGroupIndex][currentStepIndex] = StepStatus.CompletedWithError;
            AdvanceAfterStep();

            return new ScenarioEvaluation(
                action,
                true,
                ScenarioFeedbackType.Error,
                IsCompleted);
        }

        private ScenarioEvaluation ProcessSequenceViolation(
            TrainingAction action,
            int offendingStepIndex)
        {
            var steps = CurrentGroup.Steps;

            for (var stepIndex = currentStepIndex; stepIndex < steps.Count; stepIndex++)
            {
                statuses[currentGroupIndex][stepIndex] =
                    stepIndex == offendingStepIndex
                        ? StepStatus.CompletedWithError
                        : StepStatus.Skipped;
            }

            AdvanceToNextGroup();

            return new ScenarioEvaluation(
                action,
                true,
                ScenarioFeedbackType.SequenceViolation,
                IsCompleted);
        }

        private int FindFutureStep(TrainingAction action)
        {
            var steps = CurrentGroup.Steps;

            for (var stepIndex = currentStepIndex + 1; stepIndex < steps.Count; stepIndex++)
            {
                var expectedActions = steps[stepIndex].ExpectedActions;
                for (var actionIndex = 0; actionIndex < expectedActions.Count; actionIndex++)
                {
                    if (expectedActions[actionIndex].Matches(action))
                        return stepIndex;
                }
            }

            return -1;
        }

        private void AdvanceAfterStep()
        {
            currentStepIndex++;
            currentActionIndex = 0;

            if (currentStepIndex < CurrentGroup.Steps.Count)
                return;

            AdvanceToNextGroup();
        }

        private void AdvanceToNextGroup()
        {
            currentGroupIndex++;
            currentStepIndex = 0;
            currentActionIndex = 0;

            if (currentGroupIndex >= definition.Groups.Count)
                IsCompleted = true;
        }
    }
}
