using HybridLibrarySystem.Core.Model.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HybridLibrarySystem.Web.Api.V1
{
    public class AuthController : V1ApiControllerBase
    {
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Username = User.Identity.Name,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value),
                Permissions = User.Claims.Where(c => c.Type == CustomClaimTypes.Permission).Select(c => c.Value).Distinct()
            });
        }
    }
}
