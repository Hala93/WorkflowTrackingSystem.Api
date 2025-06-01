using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Data;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Models;
using WorkflowTrackingSystem.Api.Services.Interfaces;

namespace WorkflowTrackingSystem.Api.Services.Implementations
{
    public class WorkflowService : IWorkflowService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; // Use AutoMapper for DTO to Model mapping

        public WorkflowService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WorkflowResponse> CreateWorkflowAsync(CreateWorkflowRequest request)
        {
            var workflow = _mapper.Map<Workflow>(request);
            workflow.Id = Guid.NewGuid(); // Assign a new GUID

            // Ensure steps have correct WorkflowId and order
            int order = 0;
            foreach (var stepDto in request.Steps)
            {
                var step = _mapper.Map<WorkflowStep>(stepDto);
                step.Id = Guid.NewGuid();
                step.WorkflowId = workflow.Id;
                step.Order = order++;
                workflow.Steps.Add(step);
            }

            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();

            return _mapper.Map<WorkflowResponse>(workflow);
        }

        public async Task<WorkflowResponse?> GetWorkflowByIdAsync(Guid id)
        {
            var workflow = await _context.Workflows
                                         .Include(w => w.Steps.OrderBy(s => s.Order))
                                         .FirstOrDefaultAsync(w => w.Id == id);
            return _mapper.Map<WorkflowResponse>(workflow);
        }

        public async Task<IEnumerable<WorkflowResponse>> GetAllWorkflowsAsync()
        {
            var workflows = await _context.Workflows
                                          .Include(w => w.Steps.OrderBy(s => s.Order))
                                          .ToListAsync();
            return _mapper.Map<IEnumerable<WorkflowResponse>>(workflows);
        }
    }
}