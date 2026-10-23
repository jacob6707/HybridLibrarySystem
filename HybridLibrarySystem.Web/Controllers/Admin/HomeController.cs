using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Controllers.Admin
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
