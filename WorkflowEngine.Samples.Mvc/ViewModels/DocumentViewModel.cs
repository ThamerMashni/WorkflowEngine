using WorkflowEngine.Core.Models;
using WorkflowEngine.Samples.Mvc.Models;

namespace WorkflowEngine.Samples.Mvc.ViewModels
{
    public class DocumentListViewModel
    {
        public List<Document> Documents { get; set; }
        public string CurrentUserRole { get; set; }
    }

    public class DocumentDetailsViewModel
    {
        public Document Document { get; set; }
        public List<FlowStageActions> AvailableActions { get; set; }
        public List<ActionHistoryViewModel> ActionHistory { get; set; }
        public bool CanPerformActions { get; set; }
        public string CurrentUserRole { get; set; }
    }

    public class ActionHistoryViewModel
    {
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
        public string Note { get; set; }
        public string Stage { get; set; }
    }

    public class PerformActionViewModel
    {
        public int DocumentId { get; set; }
        public FlowStageActions Action { get; set; }
        public string Note { get; set; }
    }

    public class CreateDocumentViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DocumentType DocumentType { get; set; }
        public bool UseFastTrack { get; set; }
    }
}
