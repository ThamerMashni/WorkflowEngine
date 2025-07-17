using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Interfaces
{
    /// <summary>
    /// Interface for entities that have an associated workflow
    /// </summary>
    public interface IWorkflowEnabled
    {
        /// <summary>
        /// The workflow manager for this entity
        /// </summary>
        FlowManager FlowManager { get; set; }

        /// <summary>
        /// Foreign key to the FlowManager
        /// </summary>
        string FlowManagerId { get; set; }
    }

    /// <summary>
    /// Interface for entities that support multiple workflows
    /// </summary>
    public interface IMultiWorkflowEnabled
    {
        /// <summary>
        /// Collection of workflows for this entity
        /// </summary>
        List<FlowManager> FlowManagers { get; set; }
    }
}