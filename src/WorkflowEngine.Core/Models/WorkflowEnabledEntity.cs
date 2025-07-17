using WorkflowEngine.Core.Entities;
using WorkflowEngine.Core.Interfaces;

namespace WorkflowEngine.Core.Models
{
    /// <summary>
    /// Base class for entities that require workflow functionality
    /// </summary>
    public abstract class WorkflowEnabledEntity : BaseEntity, IWorkflowEnabled
    {
        public string FlowManagerId { get; set; }
        public virtual FlowManager FlowManager { get; set; }

        /// <summary>
        /// Checks if the workflow is in a specific stage
        /// </summary>
        public bool IsInStage(int stageTag)
        {
            var currentRoute = FlowManager?.GetCurrentRoute();
            var currentStage = currentRoute?.GetCurrentStage();
            return currentStage?.Tag == stageTag;
        }

        /// <summary>
        /// Checks if the workflow is in a specific route
        /// </summary>
        public bool IsInRoute(int routeTag)
        {
            var currentRoute = FlowManager?.GetCurrentRoute();
            return currentRoute?.Tag == routeTag;
        }

        /// <summary>
        /// Gets the current workflow status
        /// </summary>
        public FlowManagerStatus? GetWorkflowStatus()
        {
            return FlowManager?.Status;
        }

        /// <summary>
        /// Initializes the workflow from a template
        /// </summary>
        public void InitializeWorkflow(FlowManager flowManager)
        {
            FlowManager = flowManager;
            FlowManagerId = flowManager.Id;
        }
    }
}