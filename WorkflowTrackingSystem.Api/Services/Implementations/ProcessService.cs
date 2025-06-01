using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Data;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Enums;
using WorkflowTrackingSystem.Api.Models;
using WorkflowTrackingSystem.Api.Services.Interfaces;

namespace WorkflowTrackingSystem.Api.Services.Implementations
{
    public class ProcessService : IProcessService
    {
        private readonly AppDbContext _context;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public ProcessService(AppDbContext context, IValidationService validationService, IMapper mapper)
        {
            _context = context;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<Guid> StartProcessAsync(StartProcessRequest request)
        {
            var workflow = await _context.Workflows
                                         .Include(w => w.Steps.OrderBy(s => s.Order))
                                         .FirstOrDefaultAsync(w => w.Id == request.WorkflowId);

            if (workflow == null)
            {
                throw new ArgumentException("Workflow not found.");
            }
            if (!workflow.Steps.Any())
            {
                throw new InvalidOperationException("Workflow has no defined steps.");
            }

            var firstStep = workflow.Steps.First();

            var processInstance = new ProcessInstance
            {
                Id = Guid.NewGuid(),
                WorkflowId = request.WorkflowId,
                Initiator = request.Initiator,
                CurrentStepName = firstStep.StepName,
                Status = ProcessStatus.Active,
                InitiatedAt = DateTime.UtcNow
            };

            processInstance.StepInstances.Add(new ProcessStepInstance
            {
                Id = Guid.NewGuid(),
                StepName = firstStep.StepName,
                AssignedTo = firstStep.AssignedTo,
                Status = StepStatus.Pending,
                StartedAt = DateTime.UtcNow
            });

            _context.ProcessInstances.Add(processInstance);
            await _context.SaveChangesAsync();

            return processInstance.Id;
        }

        public async Task ExecuteStepAsync(ExecuteStepRequest request)
        {
            var processInstance = await _context.ProcessInstances
                                                .Include(pi => pi.StepInstances)
                                                .Include(pi => pi.Workflow)
                                                    .ThenInclude(w => w.Steps.OrderBy(s => s.Order))
                                                .FirstOrDefaultAsync(pi => pi.Id == request.ProcessId);

            if (processInstance == null)
            {
                throw new ArgumentException("Process instance not found.");
            }

            if (processInstance.Status != ProcessStatus.Active && processInstance.Status != ProcessStatus.Pending)
            {
                throw new InvalidOperationException($"Process is not active or pending. Current status: {processInstance.Status}");
            }

            var currentStepInstance = processInstance.StepInstances.FirstOrDefault(psi =>
                psi.StepName == request.StepName && psi.Status == StepStatus.Pending);

            if (currentStepInstance == null)
            {
                throw new InvalidOperationException($"Step '{request.StepName}' is not the current pending step for process '{request.ProcessId}'.");
            }

            // --- Validation Middleware Integration ---
            var validationResult = await _validationService.ValidateStepAsync(
                request.StepName,
                request.Action, // You might pass specific input data here instead of just action
                request.ProcessId
            );

            if (!validationResult.IsValid)
            {
                currentStepInstance.Status = StepStatus.Rejected;
                currentStepInstance.CompletedAt = DateTime.UtcNow;
                currentStepInstance.ValidationOutcome = validationResult.ErrorMessage;
                currentStepInstance.PerformedBy = request.PerformedBy; // Still record who tried
                currentStepInstance.Action = request.Action;

                processInstance.Status = ProcessStatus.Rejected; // Mark process as rejected
                await _context.SaveChangesAsync();
                throw new InvalidOperationException($"Validation failed for step '{request.StepName}': {validationResult.ErrorMessage}");
            }
            // --- End Validation Middleware Integration ---

            // Update current step instance
            currentStepInstance.Status = StepStatus.Completed;
            currentStepInstance.PerformedBy = request.PerformedBy;
            currentStepInstance.Action = request.Action;
            currentStepInstance.Comment = request.Comment;
            currentStepInstance.CompletedAt = DateTime.UtcNow;

            var workflowStepDefinition = processInstance.Workflow.Steps.FirstOrDefault(ws => ws.StepName == request.StepName);
            if (workflowStepDefinition == null)
            {
                throw new InvalidOperationException($"Workflow step definition for '{request.StepName}' not found.");
            }

            string nextStepName = workflowStepDefinition.NextStepName;

            if (nextStepName.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                processInstance.Status = ProcessStatus.Completed;
                processInstance.CompletedAt = DateTime.UtcNow;
                processInstance.CurrentStepName = "Completed"; // Mark final step
            }
            else
            {
                var nextWorkflowStep = processInstance.Workflow.Steps.FirstOrDefault(ws => ws.StepName == nextStepName);
                if (nextWorkflowStep == null)
                {
                    throw new InvalidOperationException($"Next step '{nextStepName}' not found in workflow definition.");
                }

                processInstance.CurrentStepName = nextStepName;
                processInstance.Status = ProcessStatus.Active; // Process continues being active/pending for next step

                processInstance.StepInstances.Add(new ProcessStepInstance
                {
                    Id = Guid.NewGuid(),
                    StepName = nextWorkflowStep.StepName,
                    AssignedTo = nextWorkflowStep.AssignedTo,
                    Status = StepStatus.Pending,
                    StartedAt = DateTime.UtcNow,
                    ProcessInstanceId = processInstance.Id
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProcessInstanceResponse>> GetProcessesAsync(
            Guid? workflowId = null,
            ProcessStatus? status = null,
            string? assignedTo = null)
        {
            IQueryable<ProcessInstance> query = _context.ProcessInstances
                                                        .Include(pi => pi.Workflow)
                                                        .Include(pi => pi.StepInstances.OrderBy(psi => psi.StartedAt));

            if (workflowId.HasValue)
            {
                query = query.Where(pi => pi.WorkflowId == workflowId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(pi => pi.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(assignedTo))
            {
                // This filters based on the *current* pending step being assigned to the user
                query = query.Where(pi => pi.CurrentStepName != "Completed" && pi.CurrentStepName != "Rejected" &&
                                          pi.StepInstances.Any(psi =>
                                              psi.StepName == pi.CurrentStepName &&
                                              psi.Status == StepStatus.Pending &&
                                              psi.AssignedTo == assignedTo));
            }

            var processes = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ProcessInstanceResponse>>(processes);
        }

        public async Task<ProcessInstanceResponse?> GetProcessByIdAsync(Guid id)
        {
            var process = await _context.ProcessInstances
                                        .Include(pi => pi.Workflow)
                                        .Include(pi => pi.StepInstances.OrderBy(psi => psi.StartedAt))
                                        .FirstOrDefaultAsync(pi => pi.Id == id);
            return _mapper.Map<ProcessInstanceResponse>(process);
        }
    }
}