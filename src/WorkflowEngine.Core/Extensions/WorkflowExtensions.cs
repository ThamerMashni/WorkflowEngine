using WorkflowEngine.Core.Interfaces;
using WorkflowEngine.Core.Models;
using WorkflowEngine.Core.Services;
using System.Collections.Generic;

namespace WorkflowEngine.Core.Extensions
{
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Adds workflow to any entity
        /// </summary>
        public static T WithWorkflow<T>(this T entity, FlowManager flowManager)
            where T : IWorkflowEnabled
        {
            entity.FlowManager = flowManager;
            entity.FlowManagerId = flowManager.Id;
            return entity;
        }

        /// <summary>
        /// Creates and attaches a workflow from template
        /// </summary>
        public static T WithWorkflowFromTemplate<T>(this T entity, string templateJson, IWorkflowService workflowService)
            where T : IWorkflowEnabled
        {
            var flowManager = workflowService.CreateFlowFromTemplate(templateJson);
            entity.FlowManager = flowManager;
            entity.FlowManagerId = flowManager.Id;
            return entity;
        }

        /// <summary>
        /// Performs a workflow action on the entity
        /// </summary>
        public static bool PerformWorkflowAction<T>(
            this T entity,
            FlowStageActions action,
            string note,
            string userId,
            string userName,
            IEnumerable<string> userRoles,
            IWorkflowService workflowService)
            where T : IWorkflowEnabled
        {
            if (entity.FlowManager == null)
                throw new InvalidOperationException("Entity does not have an initialized workflow");

            return workflowService.PerformAction(
                entity.FlowManager,
                action,
                note,
                userId,
                userName,
                userRoles);
        }

        /// <summary>
        /// Gets available workflow actions for the entity
        /// </summary>
        public static List<FlowStageActions> GetAvailableWorkflowActions<T>(
            this T entity,
            IEnumerable<string> userRoles,
            IWorkflowService workflowService)
            where T : IWorkflowEnabled
        {
            if (entity.FlowManager == null)
                return new List<FlowStageActions>();

            return workflowService.GetAvailableActions(entity.FlowManager, userRoles);
        }
    }
}