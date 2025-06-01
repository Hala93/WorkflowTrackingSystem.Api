using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;

namespace WorkflowTrackingSystem.Api.Services.Interfaces
{
    public interface IWorkflowService
    {
        Task<WorkflowResponse> CreateWorkflowAsync(CreateWorkflowRequest request);
        Task<WorkflowResponse?> GetWorkflowByIdAsync(Guid id);
        Task<IEnumerable<WorkflowResponse>> GetAllWorkflowsAsync();
        // Task<WorkflowResponse> UpdateWorkflowAsync(Guid id, UpdateWorkflowRequest request); // Add update logic
        // Task DeleteWorkflowAsync(Guid id); // Add delete logic
    }
}