using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pokok.IdentityServer.Application.Contracts.Identity;
using Pokok.IdentityServer.Application.Contracts.Persistence;

namespace Pokok.IdentityServer.Infrastructure.Identity;

/// <summary>
/// Middleware that resolves and sets the tenant context for each request
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ITenantContext tenantContext,
        ITenantStore tenantStore)
    {
        var tenantId = await tenantResolver.ResolveAsync();

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            // Validate tenant exists and is active
            var isValid = await tenantStore.IsValidAsync(tenantId);

            if (isValid)
            {
                tenantContext.SetTenant(tenantId);
                _logger.LogInformation("Tenant context set: {TenantId}", tenantId);
            }
            else
            {
                _logger.LogWarning("Invalid or inactive tenant: {TenantId}", tenantId);
                
                // Allow unauthenticated paths to continue (login, register, etc.)
                var path = context.Request.Path.Value?.ToLower() ?? "";
                if (!path.Contains("/identity/account/") && !path.Contains("/selecttenant"))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid or inactive tenant");
                    return;
                }
            }
        }
        else
        {
            // For paths that require a tenant, you might want to return an error
            // For now, we'll allow requests without a tenant (e.g., tenant selection page)
            _logger.LogDebug("No tenant resolved for request: {Path}", context.Request.Path);
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
