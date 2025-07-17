using Microsoft.Extensions.DependencyInjection;
using WorkflowEngine.Core.Services;

namespace WorkflowEngine.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowEngine(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowService, WorkflowService>();
            return services;
        }
    }
}