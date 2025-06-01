using System;
using System.Collections.Generic;
using WorkflowTrackingSystem.Api.Enums;

namespace WorkflowTrackingSystem.Api.Models
{
    public class ProcessInstance
    {
        public Guid Id { get; set; }

        public Guid WorkflowId { get; set; } // Foreign key to Workflow definition
        public Workflow Workflow { get; set; } = default!; // Navigation property

        public string Initiator { get; set; } = string.Empty;
        public string CurrentStepName { get; set; } = string.Empty; // The name of the current step in the workflow
        public ProcessStatus Status { get; set; } = ProcessStatus.Active;
        public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation property for step instances in this process
        public ICollection<ProcessStepInstance> StepInstances { get; set; } = new List<ProcessStepInstance>();
    }
}