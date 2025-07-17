using WorkflowEngine.Core.Models;
using WorkflowEngine.Core.Interfaces;

namespace WorkflowEngine.Samples.Mvc.Models
{
    public class Document : WorkflowEnabledEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public DocumentType DocumentType { get; set; }

        public string GetStatusBadgeClass()
        {
            return FlowManager?.Status switch
            {
                FlowManagerStatus.NotStarted => "badge bg-secondary",
                FlowManagerStatus.InProgress => "badge bg-primary",
                FlowManagerStatus.Finished => "badge bg-success",
                FlowManagerStatus.Cancelled => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }

        public string GetCurrentStageDisplay()
        {
            var route = FlowManager?.GetCurrentRoute();
            var stage = route?.GetCurrentStage();

            if (stage == null) return "Not Started";

            return stage.Tag switch
            {
                (int)DocumentStages.Draft => "Draft Creation",
                (int)DocumentStages.Review => "Under Review",
                (int)DocumentStages.Approval => "Awaiting Approval",
                (int)DocumentStages.Published => "Published",
                _ => "Unknown"
            };
        }
    }

    public enum DocumentType
    {
        Policy = 1,
        Procedure = 2,
        Report = 3,
        Contract = 4
    }

    public enum DocumentRoutes
    {
        StandardReview = 1,
        FastTrack = 2
    }

    public enum DocumentStages
    {
        Draft = 1,
        Review = 2,
        Approval = 3,
        Published = 4
    }
}