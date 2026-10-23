using Microsoft.AspNetCore.Authorization;

namespace HybridLibrarySystem.Web.Features.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
  public const string Prefix = "Permissions:";

  public RequirePermissionAttribute(params string[] permissions)
    => Policy = Prefix + string.Join(",", permissions);
}
