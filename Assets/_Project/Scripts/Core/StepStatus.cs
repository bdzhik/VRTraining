namespace VRTraining.Core
{
    /// <summary>
    /// Итог выполнения шага сценария.
    /// </summary>
    public enum StepStatus
    {
        NotStarted,
        Completed,
        CompletedWithError,
        Skipped
    }
}
