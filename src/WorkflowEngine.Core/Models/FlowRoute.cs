using WorkflowEngine.Core.Entities;

namespace WorkflowEngine.Core.Models
{
    public class FlowRoute : BaseEntity
    {
        public FlowRouteStatus Status { get; set; }
        public List<FlowStage> Stages { get; set; } = new List<FlowStage>();
        public int Order { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; } = true;
        public int Tag { get; set; } // Generic identifier for enum mapping

        public FlowStage GetCurrentStage()
        {
            return Stages?.FirstOrDefault(s => s.IsActive && s.IsCurrent);
        }

        public void StartTheFlow()
        {
            if (Stages.Any(s => s.IsCurrent && s.IsActive)) return;

            var firstStage = Stages?.Where(s => s.IsActive).OrderBy(s => s.Order).FirstOrDefault();
            if (firstStage != null)
            {
                Status = FlowRouteStatus.InProgress;
                firstStage.IsCurrent = true;
            }
        }

        public bool PerformAction(FlowStageActions action, string note, string userId, string userName)
        {
            var currentStage = GetCurrentStage();
            if (currentStage == null || !currentStage.AllowedActions.HasFlag(action))
                return false;

            // Record the action
            currentStage.Actions.Add(new FlowStageAction
            {
                ActionTaken = action,
                Note = note,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = userId,
                CreatedByUserName = userName
            });

            // Handle action consequences
            switch (action)
            {
                case FlowStageActions.Approve:
                case FlowStageActions.Create:
                case FlowStageActions.Edit:
                case FlowStageActions.Close:
                    MoveToNextStage();
                    break;
                case FlowStageActions.Deny:
                    currentStage.IsCurrent = false;
                    Status = FlowRouteStatus.Finished;
                    break;
                case FlowStageActions.RequestEdit:
                    currentStage.IsCurrent = false;
                    Status = FlowRouteStatus.NotStarted;
                    break;
            }

            return true;
        }

        private void MoveToNextStage()
        {
            var currentStage = GetCurrentStage();
            if (currentStage == null) return;

            currentStage.IsCurrent = false;

            var nextStage = Stages
                .Where(s => s.IsActive && s.Order > currentStage.Order)
                .OrderBy(s => s.Order)
                .FirstOrDefault();

            if (nextStage != null)
            {
                nextStage.IsCurrent = true;
            }
            else
            {
                Status = FlowRouteStatus.Finished;
            }
        }
    }
}