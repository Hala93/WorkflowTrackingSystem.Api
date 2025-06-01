namespace WorkflowTrackingSystem.Api.Enums
{
    public enum StepStatus
    {
        Pending,
        Completed,
        Rejected // Step was rejected (e.g., manager rejected or validation failed)
    }
}