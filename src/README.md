# WorkflowEngine

A flexible, role-based workflow management system for .NET 8+ applications.

## Features

- 🔄 **Multi-Route Workflows** - Define multiple paths through your workflow
- 📊 **Stage-Based Progression** - Clear stages with defined actions
- 🔐 **Role-Based Authorization** - Control who can perform actions at each stage
- 📝 **Action History** - Complete audit trail of all actions
- 🎯 **Template System** - Define reusable workflow templates
- 🏷️ **Tag System** - Map stages and routes to your domain enums

## Installation

```bash
dotnet add package WorkflowEngine.Core
dotnet add package WorkflowEngine.Extensions.DependencyInjection
```

## Quick Start

### 1. Define Your Enums

```csharp
public enum InvoiceRoutes
{
    StandardApproval = 1,
    FastTrackApproval = 2
}

public enum InvoiceStages
{
    Draft = 1,
    Review = 2,
    Approval = 3,
    Payment = 4
}
```

### 2. Create a Workflow Template

```csharp
var workflow = new SimpleWorkflowBuilder("Invoice Approval", "Invoice")
    .AddRoute((int)InvoiceRoutes.StandardApproval)
    .AddStage((int)InvoiceStages.Draft, 
        FlowStageActions.Create | FlowStageActions.Edit, 
        "Accountant")
    .AddStage((int)InvoiceStages.Review, 
        FlowStageActions.Approve | FlowStageActions.RequestEdit, 
        "Supervisor")
    .AddStage((int)InvoiceStages.Approval, 
        FlowStageActions.Approve | FlowStageActions.Deny, 
        "Manager")
    .Build();
```

### 3. Configure Services

```csharp
// Program.cs
builder.Services.AddWorkflowEngine();
```

### 4. Use in Your Application

```csharp
public class InvoiceService
{
    private readonly IWorkflowService _workflowService;
    
    public void ProcessInvoice(Invoice invoice, string userId, string[] userRoles)
    {
        // Perform action
        var success = _workflowService.PerformAction(
            invoice.Workflow,
            FlowStageActions.Approve,
            "Approved for payment",
            userId,
            "John Doe",
            userRoles
        );
        
        // Check available actions
        var actions = _workflowService.GetAvailableActions(
            invoice.Workflow, 
            userRoles
        );
    }
}
```

## Advanced Features

### Conditional Route Activation

```csharp
// Activate fast-track route for amounts under $1000
if (invoice.Amount < 1000)
{
    workflow.ActivateRoute((int)InvoiceRoutes.FastTrackApproval, exclusive: true);
}
```

### Route Switching

```csharp
// Switch from standard to fast-track
workflow.SwitchToRoute((int)InvoiceRoutes.FastTrackApproval);
```

## API Reference

### FlowManager
- `StartFlow()` - Initialize the workflow
- `GetCurrentRoute()` - Get the active route
- `ActivateRoute(tag, exclusive)` - Activate a specific route
- `DeactivateRoute(tag)` - Deactivate a route
- `SwitchToRoute(tag)` - Switch to a different route

### IWorkflowService
- `CreateFlowFromTemplate(json)` - Create workflow from template
- `PerformAction(...)` - Execute an action
- `GetAvailableActions(...)` - Get allowed actions for user
- `CanUserPerformAction(...)` - Check user permissions

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.