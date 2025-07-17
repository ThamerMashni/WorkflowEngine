using System.Text.Json.Serialization;

namespace WorkflowEngine.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlowManagerStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Finished = 2,
        Cancelled = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlowRouteStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Finished = 2,
        Cancelled = 3
    }

    [Flags]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlowStageActions
    {
        None = 0,
        Create = 1,
        Edit = 2,
        Approve = 4,
        Deny = 8,
        RequestEdit = 16,
        Close = 32
    }
}
