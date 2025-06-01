using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Enums;
using WorkflowTrackingSystem.Api.Services.Interfaces;

namespace WorkflowTrackingSystem.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ProcessesController : ControllerBase
    {
        private readonly IProcessService _processService;

        public ProcessesController(IProcessService processService)
        {
            _processService = processService;
        }

        // POST /v1/processes/start
        [HttpPost("start")]
        [ProducesResponseType(typeof(Guid), 200)] // Returns the Process ID
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Guid>> StartProcess([FromBody] StartProcessRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var processId = await _processService.StartProcessAsync(request);
                return Ok(processId);
            }
            catch (ArgumentException ex) // For workflow not found
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // For workflow with no steps
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while starting the process." });
            }
        }

        // POST /v1/processes/execute
        [HttpPost("execute")]
        [ProducesResponseType(204)] // No Content on success
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ExecuteStep([FromBody] ExecuteStepRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _processService.ExecuteStepAsync(request);
                return NoContent(); // 204 No Content for successful operation
            }
            catch (ArgumentException ex) // For process or step not found
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) // For validation failure or incorrect step
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while executing the step." });
            }
        }

        // GET /v1/processes
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProcessInstanceResponse>), 200)]
        public async Task<ActionResult<IEnumerable<ProcessInstanceResponse>>> GetProcesses(
            [FromQuery] Guid? workflowId,
            [FromQuery] ProcessStatus? status,
            [FromQuery] string? assignedTo)
        {
            var processes = await _processService.GetProcessesAsync(workflowId, status, assignedTo);
            return Ok(processes);
        }

        // GET /v1/processes/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProcessInstanceResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProcessInstanceResponse>> GetProcessById(Guid id)
        {
            var process = await _processService.GetProcessByIdAsync(id);
            if (process == null)
            {
                return NotFound();
            }
            return Ok(process);
        }
    }
}