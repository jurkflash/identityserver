using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pokok.IdentityServer.Application.Contracts.Identity;

namespace Pokok.IdentityServer.Infrastructure.Identity;

/// <summary>
/// Resolves tenant using multiple strategies in priority order
/// </summary>
public class CompositeTenantResolver : ITenantResolver
{
    private readonly ILogger<CompositeTenantResolver> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CompositeTenantResolver(
        ILogger<CompositeTenantResolver> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> ResolveAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null, cannot resolve tenant");
            return await Task.FromResult<string?>(null);
        }

        // Strategy 1: Header (e.g., for API calls)
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue))
        {
            var tenantId = headerValue.ToString();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                _logger.LogDebug("Tenant resolved from header: {TenantId}", tenantId);
                return tenantId;
            }
        }

        // Strategy 2: Query string (e.g., for testing or deep links)
        if (httpContext.Request.Query.TryGetValue("tenantId", out var queryValue))
        {
            var tenantId = queryValue.ToString();
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                _logger.LogDebug("Tenant resolved from query string: {TenantId}", tenantId);
                return tenantId;
            }
        }

        // Strategy 3: Subdomain (e.g., tenant1.identity.com)
        var host = httpContext.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);
        if (!string.IsNullOrWhiteSpace(subdomain))
        {
            _logger.LogDebug("Tenant resolved from subdomain: {Subdomain}", subdomain);
            return subdomain;
        }

        // Strategy 4: Cookie (for UI sessions)
        if (httpContext.Request.Cookies.TryGetValue("TenantId", out var cookieValue))
        {
            var tenantId = cookieValue;
            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                _logger.LogDebug("Tenant resolved from cookie: {TenantId}", tenantId);
                return tenantId;
            }
        }

        // Strategy 5: User claim (for authenticated requests)
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrWhiteSpace(tenantClaim))
            {
                _logger.LogDebug("Tenant resolved from user claim: {TenantId}", tenantClaim);
                return tenantClaim;
            }
        }

        _logger.LogWarning("No tenant could be resolved from request");
        return await Task.FromResult<string?>(null);
    }

    private static string? ExtractSubdomain(string host)
    {
        // Remove port if present
        var hostWithoutPort = host.Split(':')[0];

        // Split by dots
        var parts = hostWithoutPort.Split('.');

        // If we have at least 3 parts (subdomain.domain.tld), return the first part
        // Otherwise, it's likely localhost or a top-level domain
        if (parts.Length >= 3)
        {
            var subdomain = parts[0];
            // Ignore common prefixes
            if (subdomain != "www" && subdomain != "api")
            {
                return subdomain;
            }
        }

        return null;
    }
}
