using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowTrackingSystem.Api.Dtos.Request
{
    public class StartProcessRequest
    {
        [Required]
        public Guid WorkflowId { get; set; }
        [Required]
        public string Initiator { get; set; } = string.Empty; // User ID or Name
    }
}