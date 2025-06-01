using System;
using System.Collections.Generic;

namespace WorkflowTrackingSystem.Api.Dtos.Response
{
    // This DTO can be used for GET responses
    public class WorkflowStepResponse
    {
        public string StepName { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string NextStep { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}