namespace VRTraining.Core
{
    public readonly struct ScenarioResultEntry
    {
        public ScenarioResultEntry(
            int groupIndex,
            int stepIndex,
            string groupTitle,
            string stepDescription,
            StepStatus status)
        {
            GroupIndex = groupIndex;
            StepIndex = stepIndex;
            GroupTitle = groupTitle;
            StepDescription = stepDescription;
            Status = status;
        }

        public int GroupIndex { get; }
        public int StepIndex { get; }
        public string GroupTitle { get; }
        public string StepDescription { get; }
        public StepStatus Status { get; }
    }
}
