using Microsoft.AspNetCore.Authorization;

namespace HybridLibrarySystem.Web.Features.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute: AuthorizeAttribute
{
    public RequirePermissionAttribute(params string[] permissions)
        : base(policy: string.Join(",", permissions))
    {
    }
}
