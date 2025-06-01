using System;
using System.Collections.Generic;
using WorkflowTrackingSystem.Api.Enums;

namespace WorkflowTrackingSystem.Api.Dtos.Response
{
    public class ProcessInstanceResponse
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; } = string.Empty; // Add workflow name for easier display
        public string Initiator { get; set; } = string.Empty;
        public string CurrentStepName { get; set; } = string.Empty;
        public ProcessStatus Status { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<ProcessStepInstanceResponse> StepInstances { get; set; } = new List<ProcessStepInstanceResponse>();
    }

}