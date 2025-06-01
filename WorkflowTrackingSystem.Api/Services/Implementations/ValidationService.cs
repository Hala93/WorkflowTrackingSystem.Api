using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Services.Interfaces;
using WorkflowTrackingSystem.Api.Services.Implementations; // To access ValidationRulesConfig

namespace WorkflowTrackingSystem.Api.Services.Implementations
{
    public class ValidationRulesConfig
    {
        public List<ValidationRule> Rules { get; set; } = new List<ValidationRule>();
    }

    public class ValidationRule
    {
        public string StepName { get; set; } = string.Empty;
        public bool RequiresExternalValidation { get; set; }
        public string? ExternalApiUrl { get; set; } // URL for external API
        public string? ExternalApiMethod { get; set; } // GET/POST
        // Add any other specific configurations for validation (e.g., expected headers, body templates)
    }

    public class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ValidationRulesConfig _validationRules;

        public ValidationService(
            ILogger<ValidationService> logger,
            HttpClient httpClient,
            IOptions<ValidationRulesConfig> validationRulesOptions)
        {
            _logger = logger;
            _httpClient = httpClient;
            _validationRules = validationRulesOptions.Value;
        }

        public async Task<ValidationResult> ValidateStepAsync(string stepName, string action, Guid processInstanceId)
        {
            var rule = _validationRules.Rules.FirstOrDefault(r => r.StepName.Equals(stepName, StringComparison.OrdinalIgnoreCase));

            if (rule == null || !rule.RequiresExternalValidation)
            {
                // No external validation required for this step
                _logger.LogInformation($"No external validation configured for step '{stepName}'.");
                return new ValidationResult { IsValid = true };
            }

            if (string.IsNullOrEmpty(rule.ExternalApiUrl))
            {
                _logger.LogError($"Validation rule for '{stepName}' requires external validation but 'ExternalApiUrl' is not configured.");
                return new ValidationResult { IsValid = false, ErrorMessage = "Validation API URL not configured." };
            }

            _logger.LogInformation($"Performing external validation for step '{stepName}' via {rule.ExternalApiUrl}");

            try
            {
                // --- Simulate External API Call ---
                // In a real scenario, you'd build the HttpRequestMessage based on `rule.ExternalApiMethod`
                // and potentially include `action` or `processInstanceId` in the request body/query params.

                // For the assessment, let's simulate success/failure for specific scenarios
                if (stepName.Equals("Finance Approval", StringComparison.OrdinalIgnoreCase) && action.Equals("approve", StringComparison.OrdinalIgnoreCase))
                {
                    // Simulate a random success/failure or a specific scenario
                    bool simulationSuccess = new Random().Next(0, 100) > 20; // 80% chance of success
                    if (!simulationSuccess)
                    {
                        _logger.LogWarning($"Simulated external validation failed for '{stepName}' (Process ID: {processInstanceId}).");
                        return new ValidationResult { IsValid = false, ErrorMessage = "Simulated external finance check failed: Insufficient funds or invalid transaction." };
                    }
                }

                // If no specific simulation, assume success for this example
                HttpResponseMessage response;
                if (rule.ExternalApiMethod?.Equals("POST", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Example: Send a JSON payload
                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(new { ProcessId = processInstanceId, StepName = stepName, Action = action }),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );
                    response = await _httpClient.PostAsync(rule.ExternalApiUrl, content);
                }
                else // Default to GET
                {
                    response = await _httpClient.GetAsync($"{rule.ExternalApiUrl}?processId={processInstanceId}&stepName={stepName}&action={action}");
                }


                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"External validation successful for '{stepName}'.");
                    return new ValidationResult { IsValid = true };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"External validation failed for '{stepName}' with status code {response.StatusCode}. Response: {errorContent}");
                    return new ValidationResult { IsValid = false, ErrorMessage = $"External validation failed: {response.StatusCode} - {errorContent}" };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request failed during external validation for '{stepName}'.");
                return new ValidationResult { IsValid = false, ErrorMessage = $"Failed to connect to external validation service: {ex.Message}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during external validation for '{stepName}'.");
                return new ValidationResult { IsValid = false, ErrorMessage = $"An unexpected error occurred during validation: {ex.Message}" };
            }
        }
    }
}