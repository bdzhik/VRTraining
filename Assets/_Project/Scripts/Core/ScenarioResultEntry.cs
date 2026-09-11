namespace VRTraining.Core
{
    public readonly struct ScenarioResultEntry
    {
        public ScenarioResultEntry(
            int groupIndex,
            string groupTitle,
            string stepDescription,
            StepStatus status)
        {
            GroupIndex = groupIndex;
            GroupTitle = groupTitle;
            StepDescription = stepDescription;
            Status = status;
        }

        public int GroupIndex { get; }
        public string GroupTitle { get; }
        public string StepDescription { get; }
        public StepStatus Status { get; }
    }
}
