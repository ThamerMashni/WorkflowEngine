using WorkflowEngine.Core.Entities;

namespace WorkflowEngine.Core.Models
{
    public class FlowStage : BaseEntity
    {
        public int Order { get; set; }
        public int Tag { get; set; } // For enum mapping
        public List<FlowStageAction> Actions { get; set; } = new List<FlowStageAction>();
        public FlowStageActions AllowedActions { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> AllowedRoles { get; set; } = new List<string>();

        public bool CanUserAct(IEnumerable<string> userRoles)
        {
            return AllowedRoles.Any(role => userRoles.Contains(role));
        }
    }
}