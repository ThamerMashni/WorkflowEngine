using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Services
{
    public interface IWorkflowService
    {
        FlowManager CreateFlowFromTemplate(string templateJson);
        bool CanUserPerformAction(FlowManager flow, string userId, IEnumerable<string> userRoles);
        bool PerformAction(FlowManager flow, FlowStageActions action, string note, string userId, string userName, IEnumerable<string> userRoles);
        List<FlowStageActions> GetAvailableActions(FlowManager flow, IEnumerable<string> userRoles);
    }
}