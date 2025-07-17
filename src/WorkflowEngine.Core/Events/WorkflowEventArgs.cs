using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Events
{
    public class WorkflowEventArgs : EventArgs
    {
        public FlowManager FlowManager { get; set; }
        public FlowRoute Route { get; set; }
        public FlowStage Stage { get; set; }
        public FlowStageAction Action { get; set; }
    }

    public interface IWorkflowEventHandler
    {
        Task OnStageTransition(WorkflowEventArgs args);
        Task OnActionPerformed(WorkflowEventArgs args);
        Task OnWorkflowCompleted(WorkflowEventArgs args);
        Task OnRouteChanged(WorkflowEventArgs args);
    }
}