using System;
using System.Collections.Generic;

namespace WorkflowTrackingSystem.Api.Models
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


        public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
    }
}