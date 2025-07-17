using Newtonsoft.Json;
using WorkflowEngine.Core.Events;
using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IEnumerable<IWorkflowEventHandler> _eventHandlers;

        public WorkflowService(IEnumerable<IWorkflowEventHandler> eventHandlers = null)
        {
            _eventHandlers = eventHandlers ?? Enumerable.Empty<IWorkflowEventHandler>();
        }
        public FlowManager CreateFlowFromTemplate(string templateJson)
        {
            var flow = JsonConvert.DeserializeObject<FlowManager>(templateJson);

            // Generate new IDs
            flow.Id = Guid.NewGuid().ToString();
            foreach (var route in flow.Routes)
            {
                route.Id = Guid.NewGuid().ToString();
                foreach (var stage in route.Stages)
                {
                    stage.Id = Guid.NewGuid().ToString();
                    stage.Actions.Clear(); // Clear any template actions
                }
            }

            return flow;
        }

        public bool CanUserPerformAction(FlowManager flow, string userId, IEnumerable<string> userRoles)
        {
            var currentRoute = flow.GetCurrentRoute();
            var currentStage = currentRoute?.GetCurrentStage();

            return currentStage != null && currentStage.CanUserAct(userRoles);
        }

        public bool PerformAction(FlowManager flow, FlowStageActions action, string note, string userId, string userName, IEnumerable<string> userRoles)
        {
            var currentRoute = flow.GetCurrentRoute();
            if (currentRoute == null) return false;

            var currentStage = currentRoute.GetCurrentStage();
            if (currentStage == null || !currentStage.CanUserAct(userRoles))
                return false;

            var success = currentRoute.PerformAction(action, note, userId, userName);

            // Check if route is finished and move to next route
            if (success && currentRoute.Status == FlowRouteStatus.Finished)
            {
                currentRoute.IsCurrent = false;
                var nextRoute = flow.Routes
                    .Where(r => r.IsActive && r.Order > currentRoute.Order)
                    .OrderBy(r => r.Order)
                    .FirstOrDefault();

                if (nextRoute != null)
                {
                    nextRoute.IsCurrent = true;
                    nextRoute.StartTheFlow();
                }
                else
                {
                    flow.Status = FlowManagerStatus.Finished;
                }
            }

            return success;
        }

        public List<FlowStageActions> GetAvailableActions(FlowManager flow, IEnumerable<string> userRoles)
        {
            var currentRoute = flow.GetCurrentRoute();
            var currentStage = currentRoute?.GetCurrentStage();

            if (currentStage == null || !currentStage.CanUserAct(userRoles))
                return new List<FlowStageActions>();

            return Enum.GetValues<FlowStageActions>()
                .Where(a => a != FlowStageActions.None && currentStage.AllowedActions.HasFlag(a))
                .ToList();
        }

        private async Task RaiseEvent(Func<IWorkflowEventHandler, Task> eventAction)
        {
            foreach (var handler in _eventHandlers)
            {
                await eventAction(handler);
            }
        }
    }
}