using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowTrackingSystem.Api.Models
{
    public class WorkflowStep
    {
        public Guid Id { get; set; }

        public Guid WorkflowId { get; set; } // Foreign key
        public Workflow Workflow { get; set; } = default!; // Navigation property

        public string StepName { get; set; } = string.Empty;// e.g., "Submit Request", "Manager Approval"
        public string AssignedTo { get; set; } = string.Empty; // e.g., "employee", "manager", "finance"
        public string ActionType { get; set; } = string.Empty; // e.g., "input", "approve_reject"
        public string NextStepName { get; set; } = string.Empty; // Name of the next logical step (e.g., "Manager Approval" or "Completed")
        public int Order { get; set; } // To define the sequence of steps
        //public bool RequiresValidation { get; set; } = false; // Indicates if this step needs external validation
        //public string ValidationApiEndpoint { get; set; } // Optional: URL for external validation API
    }
}