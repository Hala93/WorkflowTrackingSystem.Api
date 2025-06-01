using AutoMapper;
using WorkflowTrackingSystem.Api.Dtos.Request;
using WorkflowTrackingSystem.Api.Dtos.Response;
using WorkflowTrackingSystem.Api.Models;

namespace WorkflowTrackingSystem.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Workflow Mappings
            CreateMap<CreateWorkflowRequest, Workflow>()
                .ForMember(dest => dest.Steps, opt => opt.Ignore()); // Steps are mapped manually in service
            CreateMap<WorkflowStepDto, WorkflowStep>();
            CreateMap<Workflow, WorkflowResponse>();
            CreateMap<WorkflowStep, WorkflowStepResponse>()
                .ForMember(dest => dest.NextStep, opt => opt.MapFrom(src => src.NextStepName)); // Map NextStepName to NextStep

            // Process Mappings
            CreateMap<ProcessInstance, ProcessInstanceResponse>()
                .ForMember(dest => dest.WorkflowName, opt => opt.MapFrom(src => src.Workflow.Name)); // Include workflow name
            CreateMap<ProcessStepInstance, ProcessStepInstanceResponse>();
        }
    }
}