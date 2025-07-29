# WorkflowEngine.Extensions.DependencyInjection

Dependency injection extensions for WorkflowEngine. Provides easy integration with ASP.NET Core applications.

## Installation

```bash
dotnet add package WorkflowEngine.Extensions.DependencyInjection
```

## Usage

### Basic Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add WorkflowEngine services
builder.Services.AddWorkflowEngine();

var app = builder.Build();
```

### What Gets Registered

The `AddWorkflowEngine()` extension method registers:
- `IWorkflowService` - Core workflow operations
- `IAuditService` - Audit trail functionality

### Using in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    
    public DocumentController(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    
    [HttpPost("{id}/approve")]
    public IActionResult Approve(int id, [FromBody] ApprovalRequest request)
    {
        var document = GetDocument(id);
        var success = _workflowService.PerformAction(
            document.FlowManager,
            FlowStageActions.Approve,
            request.Note,
            User.Identity.Name,
            User.Identity.Name,
            GetUserRoles()
        );
        
        return success ? Ok() : BadRequest();
    }
}
```

## Requirements

- .NET 8.0 or later
- WorkflowEngine.Core package
