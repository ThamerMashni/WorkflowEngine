using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Samples.Mvc.Services;
using WorkflowEngine.Samples.Mvc.ViewModels;

namespace WorkflowEngine.Samples.Mvc.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            var viewModel = new DocumentListViewModel
            {
                Documents = documents,
                CurrentUserRole = GetCurrentUserRole()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var document = await _documentService.GetDocumentAsync(id);
            if (document == null) return NotFound();

            var userRoles = GetUserRoles();
            var viewModel = new DocumentDetailsViewModel
            {
                Document = document,
                AvailableActions = _documentService.GetAvailableActions(document, userRoles),
                ActionHistory = _documentService.GetActionHistory(document),
                CanPerformActions = _documentService.GetAvailableActions(document, userRoles).Any(),
                CurrentUserRole = GetCurrentUserRole()
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var document = await _documentService.CreateDocumentAsync(
                model,
                GetUserId(),
                GetUserName()
            );

            TempData["Success"] = "Document created successfully!";
            return RedirectToAction(nameof(Details), new { id = document.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformAction(PerformActionViewModel model)
        {
            var success = await _documentService.PerformActionAsync(
                model.DocumentId,
                model.Action,
                model.Note,
                GetUserId(),
                GetUserName(),
                GetUserRoles()
            );

            if (success)
            {
                TempData["Success"] = $"Action {model.Action} performed successfully!";
            }
            else
            {
                TempData["Error"] = "Unable to perform the requested action.";
            }

            return RedirectToAction(nameof(Details), new { id = model.DocumentId });
        }

        private string GetUserId() => User.Identity?.Name ?? "demo-user";
        private string GetUserName() => User.Identity?.Name ?? "Demo User";
        private string GetCurrentUserRole() => HttpContext.Session.GetString("UserRole") ?? "Author";
        private string[] GetUserRoles() => new[] { GetCurrentUserRole() };
    }
}
