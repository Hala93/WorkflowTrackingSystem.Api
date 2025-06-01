using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Enums;

namespace WorkflowTrackingSystem.Api.Services.Interfaces
{
    public interface IProcessService
    {
        Task<Guid> StartProcessAsync(StartProcessRequest request);
        Task ExecuteStepAsync(ExecuteStepRequest request);
        Task<IEnumerable<ProcessInstanceResponse>> GetProcessesAsync(
            Guid? workflowId = null,
            ProcessStatus? status = null,
            string? assignedTo = null);
        Task<ProcessInstanceResponse?> GetProcessByIdAsync(Guid id);
    }
}