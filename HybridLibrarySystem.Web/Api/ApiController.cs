using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Api;

[Route("api")]
public class ApiController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("API is running");
    }
}
