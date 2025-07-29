# WorkflowEngine

A flexible, role-based workflow management system for .NET 8+ applications.

[![NuGet](https://img.shields.io/nuget/v/WorkflowEngine.Core.svg)](https://www.nuget.org/packages/WorkflowEngine.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

- **Multi-Route Workflows** - Define multiple paths through your workflow
- **Stage-Based Progression** - Clear stages with defined actions
- **Role-Based Authorization** - Control who can perform actions at each stage
- **Action History** - Complete audit trail of all actions
- **Template System** - Define reusable workflow templates
- **Tag System** - Map stages and routes to your domain enums
- **Model Integration** - Embed workflows directly in your domain models

## Installation

```bash
dotnet add package WorkflowEngine.Core
dotnet add package WorkflowEngine.Extensions.DependencyInjection
```

## Quick Start

```csharp
// 1. Define your workflow
var workflow = new SimpleWorkflowBuilder("Document Approval", "Document")
    .AddRoute(1)
    .AddStage(1, FlowStageActions.Create | FlowStageActions.Edit, "Author")
    .AddStage(2, FlowStageActions.Approve | FlowStageActions.Deny, "Manager")
    .Build();

// 2. Add to your services
builder.Services.AddWorkflowEngine();

// 3. Use in your models
public class Document : WorkflowEnabledEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
}
```

## Documentation

See the [full documentation](src/README.md) for detailed usage and examples.

## Sample Application

Check out the [MVC Sample](WorkflowEngine.Samples.Mvc/) to see the workflow engine in action.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.
