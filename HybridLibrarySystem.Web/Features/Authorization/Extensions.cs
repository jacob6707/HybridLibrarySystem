using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Core.Model.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HybridLibrarySystem.Web.Features.Authorization;

public static class Extensions
{
    public static async Task SeedRolesAndPermissions(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            await EnsureRolePermissionsAsync(roleManager, logger, Roles.Admin, Permissions.GetPermissions());
            await EnsureRolePermissionsAsync(roleManager, logger, Roles.Librarian,
            [
                Permissions.CreateAuthor,
                Permissions.UpdateAuthor,
                Permissions.ReadAuthor,
                Permissions.DeleteAuthor,
                Permissions.CreateBook,
                Permissions.UpdateBook,
                Permissions.ReadBook,
                Permissions.DeleteBook,
                Permissions.ReserveBook,
                Permissions.CreateCategory,
                Permissions.UpdateCategory,
                Permissions.ReadCategory,
                Permissions.DeleteCategory,
                Permissions.ReadMember
            ]);
            await EnsureRolePermissionsAsync(roleManager, logger, Roles.Member,
            [
                Permissions.ReadBook,
                Permissions.ReserveBook,
                Permissions.ReadAuthor,
                Permissions.ReadCategory,
                Permissions.ReadMember
            ]);
            if (!await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Creating default admin user");
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FirstName = "Default",
                    LastName = "Admin"
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
            }
        }
    }
    
    private static async Task EnsureRolePermissionsAsync(RoleManager<IdentityRole> roleManager, ILogger logger, string roleName, IEnumerable<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            logger.LogInformation("Creating role: {Role}", roleName);
            role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
        }

        var existing = (await roleManager.GetClaimsAsync(role))
            .Where(c => c.Type == CustomClaimTypes.Permission)
            .Select(c => c.Value)
            .ToHashSet();
        foreach (var permission in permissions.Where(p => !existing.Contains(p)))
        {
            logger.LogInformation("Granting permission {Permission} to role {Role}", permission, roleName);
            await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        }
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        var builder = services.AddAuthorizationBuilder();
        foreach (var permission in Permissions.GetPermissions())
        {
            builder.AddPolicy(permission, policy => policy.RequireClaim(CustomClaimTypes.Permission, permission));
        }
        return services;
    }
}
