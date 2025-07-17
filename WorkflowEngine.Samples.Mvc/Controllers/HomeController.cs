using Microsoft.AspNetCore.Mvc;

namespace WorkflowEngine.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Set default role if not set
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                HttpContext.Session.SetString("UserRole", "Author");
            }

            return View();
        }

        [HttpPost]
        public IActionResult SwitchRole(string role)
        {
            HttpContext.Session.SetString("UserRole", role);
            TempData["Success"] = $"Switched to {role} role";
            return RedirectToAction("Index", "Documents");
        }
    }
}