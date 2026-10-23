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
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole is null)
            {
                logger.LogInformation("Creating role: {Role}", Roles.Admin);
                await roleManager.CreateAsync(adminRole = new IdentityRole(Roles.Admin));

                foreach (var permission in Permissions.GetPermissions())
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, permission));
                }
            }
            var librarianRole = await roleManager.FindByNameAsync(Roles.Librarian);
            if (librarianRole is null)
            {
                logger.LogInformation("Creating role: {Role}", Roles.Librarian);
                await roleManager.CreateAsync(librarianRole = new IdentityRole(Roles.Librarian));

                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.CreateAuthor));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.UpdateAuthor));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadAuthor));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.DeleteAuthor));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.CreateBook));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.UpdateBook));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadBook));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.DeleteBook));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.CreateCategory));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.UpdateCategory));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadCategory));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.DeleteCategory));
                await roleManager.AddClaimAsync(librarianRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadMember));
            }
            var memberRole = await roleManager.FindByNameAsync(Roles.Member);
            if (memberRole is null)
            {
                logger.LogInformation("Creating role: {Role}", Roles.Member);
                await roleManager.CreateAsync(memberRole = new IdentityRole(Roles.Member));

                await roleManager.AddClaimAsync(memberRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadBook));
                await roleManager.AddClaimAsync(memberRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadAuthor));
                await roleManager.AddClaimAsync(memberRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadCategory));
                await roleManager.AddClaimAsync(memberRole, new Claim(CustomClaimTypes.Permission, Permissions.ReadMember));
            }
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
    
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        var builder = services.AddAuthorizationBuilder();
        foreach (var Permission in Permissions.GetPermissions())
        {
            builder.AddPolicy(Permission, policy => policy.RequireClaim(CustomClaimTypes.Permission, Permission));
        }
        return services;
    }
}
