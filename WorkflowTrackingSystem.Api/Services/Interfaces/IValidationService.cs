using System;
using System.Threading.Tasks;

namespace WorkflowTrackingSystem.Api.Services.Interfaces
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        // public object? ValidationDetails { get; set; } // For richer error info
    }

    public interface IValidationService
    {
        Task<ValidationResult> ValidateStepAsync(string stepName, string action, Guid processInstanceId);
    }
}