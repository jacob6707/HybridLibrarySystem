using HybridLibrarySystem.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;
using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Web.Features.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using HybridLibrarySystem.Web.Api.Middleware;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LibraryContext") ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.");

builder.Services.AddDataServices(options =>
{
    options.UseSqlServer(connectionString);
    //options.LogTo(Console.WriteLine, minimumLevel: LogLevel.Trace);
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Matches the key lengths used by the existing AddIdentity migration.
    options.Stores.MaxLengthForKeys = 128;
}).AddEntityFrameworkStores<LibraryContext>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddAuthorizationPolicies();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LoginPath = "/Identity/Account/Login";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddOpenApi();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = false;
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Api", policy => policy
        .WithOrigins("http://localhost:5173").AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Mapping OpenAPI");
    app.MapOpenApi();
    app.MapScalarApiReference("/api/docs", options =>
    {
        options.WithTitle("Hybrid Library System API Reference");
    });
    app.Logger.LogInformation("Mapped Scalar API Reference");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.SeedRolesAndPermissions();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseAntiforgeryCookie();

app.MapFallbackToFile("/app/{*path:nonfile}", "/app/index.html");

app.MapControllers();

app.MapRazorPages();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
