using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Services.Interfaces;

namespace WorkflowTrackingSystem.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class WorkflowsController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public WorkflowsController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        // POST /v1/workflows
        [HttpPost]
        [ProducesResponseType(typeof(WorkflowResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<WorkflowResponse>> CreateWorkflow([FromBody] CreateWorkflowRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var workflow = await _workflowService.CreateWorkflowAsync(request);
                return CreatedAtAction(nameof(GetWorkflowById), new { id = workflow.Id }, workflow);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while creating the workflow." });
            }
        }

        // GET /v1/workflows/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkflowResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WorkflowResponse>> GetWorkflowById(Guid id)
        {
            var workflow = await _workflowService.GetWorkflowByIdAsync(id);
            if (workflow == null)
            {
                return NotFound();
            }
            return Ok(workflow);
        }

        // GET /v1/workflows
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WorkflowResponse>), 200)]
        public async Task<ActionResult<IEnumerable<WorkflowResponse>>> GetAllWorkflows()
        {
            var workflows = await _workflowService.GetAllWorkflowsAsync();
            return Ok(workflows);
        }

        // TODO: Implement PUT and DELETE endpoints for workflows
    }
}