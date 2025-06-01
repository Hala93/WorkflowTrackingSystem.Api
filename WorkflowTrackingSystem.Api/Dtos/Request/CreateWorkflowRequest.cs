using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowTrackingSystem.Api.Dtos.Request
{
    public class CreateWorkflowRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A workflow must have at least one step.")]
        public List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
    }

    public class WorkflowStepDto
    {
        [Required]
        public string StepName { get; set; } = string.Empty;
        [Required]
        public string AssignedTo { get; set; } = string.Empty;
        [Required]
        public string ActionType { get; set; } = string.Empty;
        [Required]
        public string NextStep { get; set; } = string.Empty;
    }
}