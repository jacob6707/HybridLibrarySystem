using Microsoft.AspNetCore.Antiforgery;

namespace HybridLibrarySystem.Web.Api.Middleware;

public class AntiforgeryCookieMiddleware
{
    private readonly RequestDelegate _next;

    public AntiforgeryCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        var tokens = antiforgery.GetAndStoreTokens(context);

        if (tokens.RequestToken != null)
        {
            context.Response.Cookies.Append(
                "XSRF-TOKEN",
                tokens.RequestToken,
                new CookieOptions { HttpOnly = false, Path = "/" }
            );
        }

        await _next(context);
    }
}
public static class AntiforgeryCookieMiddlewareExtensions
{
    public static IApplicationBuilder UseAntiforgeryCookie(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AntiforgeryCookieMiddleware>();
    }
}