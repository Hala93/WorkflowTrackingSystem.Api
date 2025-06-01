using System;
using System.Collections.Generic;
using WorkflowTrackingSystem.Api.Enums;

namespace WorkflowTrackingSystem.Api.Dtos.Response
{
    public class ProcessStepInstanceResponse
    {
        public string StepName { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string? PerformedBy { get; set; }
        public string? Action { get; set; }
        public StepStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comment { get; set; }
        public string? ValidationOutcome { get; set; }
    }
}