using HybridLibrarySystem.Core.Model.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace HybridLibrarySystem.Web.Features.Authorization;
public class PermissionAuthorizationRequirement(params string[] allowedPermissions) : IAuthorizationRequirement
{
  public string[] AllowedPermissions { get; } = allowedPermissions;
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
  protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
  {
    if (requirement.AllowedPermissions.Any(p =>
          context.User.HasClaim(CustomClaimTypes.Permission, p)))
    {
      context.Succeed(requirement);
    }
    return Task.CompletedTask;
  }
}