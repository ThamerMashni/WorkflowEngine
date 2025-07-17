using WorkflowEngine.Core.Services;
using WorkflowEngine.Core.Builders;
using WorkflowEngine.Core.Models;
using WorkflowEngine.Core.Extensions;
using WorkflowEngine.Samples.Mvc.Models;
using WorkflowEngine.Samples.Mvc.ViewModels;
using WorkflowEngine.Samples.Mvc.Data;

namespace WorkflowEngine.Samples.Mvc.Services
{
    public interface IDocumentService
    {
        Task<List<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentAsync(int id);
        Task<Document> CreateDocumentAsync(CreateDocumentViewModel model, string userId, string userName);
        Task<bool> PerformActionAsync(int documentId, FlowStageActions action, string note, string userId, string userName, string[] roles);
        List<FlowStageActions> GetAvailableActions(Document document, string[] roles);
        List<ActionHistoryViewModel> GetActionHistory(Document document);
    }

    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly IWorkflowService _workflowService;

        public DocumentService(IDocumentRepository repository, IWorkflowService workflowService)
        {
            _repository = repository;
            _workflowService = workflowService;
        }

        public async Task<Document> CreateDocumentAsync(CreateDocumentViewModel model, string userId, string userName)
        {
            var document = new Document
            {
                Title = model.Title,
                Content = model.Content,
                DocumentType = model.DocumentType,
                Author = userName,
                CreatedDate = DateTime.UtcNow
            };

            // Create workflow based on fast track selection
            var template = model.UseFastTrack
                ? CreateFastTrackTemplate()
                : CreateStandardTemplate();

            document.WithWorkflowFromTemplate(template, _workflowService);
            document.FlowManager.StartFlow();

            await _repository.AddAsync(document);
            return document;
        }

        public async Task<bool> PerformActionAsync(int documentId, FlowStageActions action, string note, string userId, string userName, string[] roles)
        {
            var document = await _repository.GetByIdAsync(documentId);
            if (document == null) return false;

            var success = _workflowService.PerformAction(
                document.FlowManager,
                action,
                note,
                userId,
                userName,
                roles
            );

            if (success)
            {
                await _repository.UpdateAsync(document);
            }

            return success;
        }

        public List<FlowStageActions> GetAvailableActions(Document document, string[] roles)
        {
            return _workflowService.GetAvailableActions(document.FlowManager, roles);
        }

        public List<ActionHistoryViewModel> GetActionHistory(Document document)
        {
            var history = new List<ActionHistoryViewModel>();

            foreach (var route in document.FlowManager.Routes)
            {
                foreach (var stage in route.Stages)
                {
                    history.AddRange(stage.Actions.Select(a => new ActionHistoryViewModel
                    {
                        Action = a.ActionTaken.ToString(),
                        PerformedBy = a.CreatedByUserName,
                        PerformedAt = a.CreatedDate,
                        Note = a.Note,
                        Stage = GetStageName(stage.Tag)
                    }));
                }
            }

            return history.OrderByDescending(h => h.PerformedAt).ToList();
        }
        public async Task<List<Document>> GetAllDocumentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Document> GetDocumentAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        private string CreateStandardTemplate()
        {
            return new SimpleWorkflowBuilder("Document Review", "Document")
                .AddRoute((int)DocumentRoutes.StandardReview)
                .AddStage((int)DocumentStages.Draft,
                    FlowStageActions.Create | FlowStageActions.Edit,
                    "Author")
                .AddStage((int)DocumentStages.Review,
                    FlowStageActions.Approve | FlowStageActions.RequestEdit,
                    "Reviewer")
                .AddStage((int)DocumentStages.Approval,
                    FlowStageActions.Approve | FlowStageActions.Deny | FlowStageActions.RequestEdit,
                    "Manager")
                .AddStage((int)DocumentStages.Published,
                    FlowStageActions.Close,
                    "Publisher")
                .BuildTemplateJson();
        }

        private string CreateFastTrackTemplate()
        {
            return new SimpleWorkflowBuilder("Fast Track Review", "Document")
                .AddRoute((int)DocumentRoutes.FastTrack)
                .AddStage((int)DocumentStages.Draft,
                    FlowStageActions.Create | FlowStageActions.Edit,
                    "Author")
                .AddStage((int)DocumentStages.Approval,
                    FlowStageActions.Approve | FlowStageActions.Deny,
                    "Manager")
                .BuildTemplateJson();
        }

        private string GetStageName(int tag)
        {
            return tag switch
            {
                (int)DocumentStages.Draft => "Draft",
                (int)DocumentStages.Review => "Review",
                (int)DocumentStages.Approval => "Approval",
                (int)DocumentStages.Published => "Published",
                _ => "Unknown"
            };
        }
    }
}