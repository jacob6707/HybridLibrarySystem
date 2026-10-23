using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    [RequirePermission(Permissions.CreateAuthor)]
  public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
