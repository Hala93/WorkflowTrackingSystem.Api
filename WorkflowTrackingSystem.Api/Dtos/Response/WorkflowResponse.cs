using System;
using System.Collections.Generic;

namespace WorkflowTrackingSystem.Api.Dtos.Response
{
    // This DTO can be used for GET responses
    public class WorkflowResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WorkflowStepResponse> Steps { get; set; } = new List<WorkflowStepResponse>();
    }
}