using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowTrackingSystem.Api.Dtos.Request
{
    public class ExecuteStepRequest
    {
        [Required]
        public Guid ProcessId { get; set; }
        [Required]
        public string StepName { get; set; } = string.Empty; // The name of the step being executed
        [Required]
        public string PerformedBy { get; set; } = string.Empty; // User ID or Name
        [Required]
        public string Action { get; set; } = string.Empty; // e.g., "approve", "reject", "submit"
        public string? Comment { get; set; }
        // You might add specific input data for "input" action_type steps
        // public object? InputData { get; set; }
    }
}