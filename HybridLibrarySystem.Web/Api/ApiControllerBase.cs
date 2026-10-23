using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Api;

[ApiController]
[Route("api/[controller]")]
[EnableCors("Api")]
public abstract class ApiControllerBase : ControllerBase {}
