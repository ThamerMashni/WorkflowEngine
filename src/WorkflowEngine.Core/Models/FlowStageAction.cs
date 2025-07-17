using WorkflowEngine.Core.Entities;

namespace WorkflowEngine.Core.Models
{
    public class FlowStageAction : BaseEntity
    {
        public string Note { get; set; }
        public FlowStageActions ActionTaken { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
    }
}