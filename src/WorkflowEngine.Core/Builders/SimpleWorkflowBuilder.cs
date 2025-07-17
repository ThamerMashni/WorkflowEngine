using Newtonsoft.Json;
using WorkflowEngine.Core.Models;

namespace WorkflowEngine.Core.Builders
{
    public class SimpleWorkflowBuilder
    {
        private readonly FlowManager _flow = new FlowManager();
        private FlowRoute _currentRoute;

        public SimpleWorkflowBuilder(string name, string type)
        {
            _flow.Name = name;
            _flow.Type = type;
            _flow.Status = FlowManagerStatus.NotStarted;
            _flow.IsActive = true;
        }

        public SimpleWorkflowBuilder AddRoute(int tag)
        {
            _currentRoute = new FlowRoute
            {
                Tag = tag,
                Order = _flow.Routes.Count + 1,
                IsActive = true,
                Status = FlowRouteStatus.NotStarted
            };
            _flow.Routes.Add(_currentRoute);
            return this;
        }

        public SimpleWorkflowBuilder AddStage(int tag, FlowStageActions allowedActions, params string[] roles)
        {
            if (_currentRoute == null)
                throw new InvalidOperationException("Must add a route before adding stages");

            var stage = new FlowStage
            {
                Tag = tag,
                Order = _currentRoute.Stages.Count,
                AllowedActions = allowedActions,
                AllowedRoles = roles.ToList(),
                IsActive = true
            };

            _currentRoute.Stages.Add(stage);
            return this;
        }

        public FlowManager Build()
        {
            return _flow;
        }

        public string BuildTemplateJson()
        {
            return JsonConvert.SerializeObject(_flow, Formatting.Indented);
        }
    }

    // Example templates
    public static class WorkflowTemplates
    {
        // Example enums that developers would define for their domain
        private enum GenericRouteTags
        {
            MainRoute = 1,
            AlternateRoute = 2
        }

        private enum GenericStageTags
        {
            CreateStage = 1,
            ReviewStage = 2,
            ApprovalStage = 3,
            FinalStage = 4
        }

        public static string SimpleApproval()
        {
            return new SimpleWorkflowBuilder("Simple Approval", "Approval")
                .AddRoute((int)GenericRouteTags.MainRoute)
                .AddStage((int)GenericStageTags.CreateStage, FlowStageActions.Create | FlowStageActions.Edit, "Author")
                .AddStage((int)GenericStageTags.ApprovalStage, FlowStageActions.Approve | FlowStageActions.Deny | FlowStageActions.RequestEdit, "Approver")
                .BuildTemplateJson();
        }

        public static string DocumentReview()
        {
            return new SimpleWorkflowBuilder("Document Review", "Document")
                .AddRoute((int)GenericRouteTags.MainRoute)
                .AddStage((int)GenericStageTags.CreateStage, FlowStageActions.Create | FlowStageActions.Edit, "Author")
                .AddStage((int)GenericStageTags.ReviewStage, FlowStageActions.Approve | FlowStageActions.RequestEdit, "Reviewer")
                .AddStage((int)GenericStageTags.ApprovalStage, FlowStageActions.Approve | FlowStageActions.Deny, "Manager")
                .BuildTemplateJson();
        }

        // Example showing how a developer would create domain-specific workflow
        public static string ProjectApproval()
        {
            // Developer would define these enums in their project
            const int ApprovalRoute = 1;
            const int CharterRoute = 2;
            const int CreateEditStage = 1;
            const int OwnerReviewStage = 2;
            const int PortfolioReviewStage = 3;
            const int FinalApprovalStage = 4;

            return new SimpleWorkflowBuilder("Project Approval", "Project")
                .AddRoute(ApprovalRoute)
                .AddStage(CreateEditStage, FlowStageActions.Create | FlowStageActions.Edit, "ProjectManager")
                .AddStage(OwnerReviewStage, FlowStageActions.Approve | FlowStageActions.RequestEdit | FlowStageActions.Deny, "ProjectOwner")
                .AddStage(PortfolioReviewStage, FlowStageActions.Approve | FlowStageActions.RequestEdit | FlowStageActions.Deny, "PortfolioManager")
                .AddStage(FinalApprovalStage, FlowStageActions.Approve | FlowStageActions.Deny, "Executive")
                .AddRoute(CharterRoute)
                .AddStage(CreateEditStage, FlowStageActions.Create | FlowStageActions.Edit, "ProjectManager")
                .AddStage(OwnerReviewStage, FlowStageActions.Approve | FlowStageActions.RequestEdit, "ProjectOwner")
                .BuildTemplateJson();
        }
    }
}