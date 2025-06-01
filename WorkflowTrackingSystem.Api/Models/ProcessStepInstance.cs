using System;
using WorkflowTrackingSystem.Api.Enums;

namespace WorkflowTrackingSystem.Api.Models
{
    public class ProcessStepInstance
    {
        public Guid Id { get; set; }

        public Guid ProcessInstanceId { get; set; } // Foreign key
        public ProcessInstance ProcessInstance { get; set; } = default!; // Navigation property

        public string StepName { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty; // Role/User assigned to this step
        public string? PerformedBy { get; set; } // User who performed the action
        public string? Action { get; set; } // e.g., "approve", "reject", "submit"
        public StepStatus Status { get; set; } = StepStatus.Pending;
        public DateTime? StartedAt { get; set; } // When this specific step became active/pending
        public DateTime? CompletedAt { get; set; } // When this specific step was completed/rejected

        public string? Comment { get; set; } // Optional: comments on step execution
        public string? ValidationOutcome { get; set; } // Store validation results/error message
    }
}