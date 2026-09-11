namespace VRTraining.Core
{
    /// <summary>
    /// Результат обработки одного действия пользователя.
    /// </summary>
    public readonly struct ScenarioEvaluation
    {
        public ScenarioEvaluation(
            TrainingAction action,
            bool wasProcessed,
            ScenarioFeedbackType feedback,
            bool stepCompleted,
            bool groupCompleted,
            bool scenarioCompleted)
        {
            Action = action;
            WasProcessed = wasProcessed;
            Feedback = feedback;
            StepCompleted = stepCompleted;
            GroupCompleted = groupCompleted;
            ScenarioCompleted = scenarioCompleted;
        }

        public TrainingAction Action { get; }
        public bool WasProcessed { get; }
        public ScenarioFeedbackType Feedback { get; }
        public bool StepCompleted { get; }
        public bool GroupCompleted { get; }
        public bool ScenarioCompleted { get; }

        public static ScenarioEvaluation Ignored(TrainingAction action, bool scenarioCompleted)
        {
            return new ScenarioEvaluation(
                action,
                false,
                ScenarioFeedbackType.None,
                false,
                false,
                scenarioCompleted);
        }
    }
}
