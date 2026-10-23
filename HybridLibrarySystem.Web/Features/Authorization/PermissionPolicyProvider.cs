using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace HybridLibrarySystem.Web.Features.Authorization;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
  : DefaultAuthorizationPolicyProvider(options)
{
  public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
  {
    if (policyName.StartsWith(RequirePermissionAttribute.Prefix, StringComparison.Ordinal))
    {
      var perms = policyName[RequirePermissionAttribute.Prefix.Length..]
        .Split(',', StringSplitOptions.RemoveEmptyEntries);

      return new AuthorizationPolicyBuilder()
        .AddRequirements(new PermissionAuthorizationRequirement(perms))
        .Build();
    }
    return await base.GetPolicyAsync(policyName);
  }
}