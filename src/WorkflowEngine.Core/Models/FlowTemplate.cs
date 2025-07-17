using WorkflowEngine.Core.Entities;

namespace WorkflowEngine.Core.Models
{
    public class FlowTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string TemplateJson { get; set; }
    }
}