using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Domain.Entities;

namespace Pokok.IdentityServer.Infrastructure.Identity;

public static class IdentitySeed
{
    public const string AdminRoleName = "Admin";
    public const string UserRoleName = "User";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IdentitySeed");
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        try
        {
            await SeedTenantsAsync(scope.ServiceProvider, configuration, logger);
            await SeedRolesAsync(scope.ServiceProvider, logger);
            await SeedAdminUserAsync(scope.ServiceProvider, configuration, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding identity data");
            throw;
        }
    }

    private static async Task SeedTenantsAsync(IServiceProvider serviceProvider, IConfiguration configuration, ILogger logger)
    {
        var seedConfig = configuration.GetSection("SeedData:DefaultTenant");
        
        var tenantId = seedConfig["Id"] ?? "default-tenant";
        var tenantName = seedConfig["Name"] ?? "Default Tenant";
        var subdomain = seedConfig["Subdomain"] ?? "default";

        var tenantStore = serviceProvider.GetRequiredService<ITenantStore>();

        var existingTenant = await tenantStore.GetByIdAsync(tenantId);
        if (existingTenant == null)
        {
            var defaultTenant = new Tenant
            {
                Id = tenantId,
                Name = tenantName,
                Subdomain = subdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await tenantStore.CreateAsync(defaultTenant);
            logger.LogInformation("Default tenant '{TenantName}' created", tenantName);
        }
        else
        {
            logger.LogDebug("Default tenant already exists");
        }
    }

    private static async Task SeedRolesAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<PokokRole>>();

        // Seed Admin role
        if (!await roleManager.RoleExistsAsync(AdminRoleName))
        {
            var adminRole = new PokokRole { Name = AdminRoleName };
            var result = await roleManager.CreateAsync(adminRole);
            
            if (result.Succeeded)
                logger.LogInformation("Role '{RoleName}' created", AdminRoleName);
            else
                logger.LogError("Failed to create role '{RoleName}': {Errors}", 
                    AdminRoleName, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Seed User role
        if (!await roleManager.RoleExistsAsync(UserRoleName))
        {
            var userRole = new PokokRole { Name = UserRoleName };
            var result = await roleManager.CreateAsync(userRole);
            
            if (result.Succeeded)
                logger.LogInformation("Role '{RoleName}' created", UserRoleName);
        }
    }

    private static async Task SeedAdminUserAsync(IServiceProvider serviceProvider, IConfiguration configuration, ILogger logger)
    {
        var seedConfig = configuration.GetSection("SeedData:AdminUser");
        
        var adminEmail = seedConfig["Email"];
        var adminPassword = seedConfig["Password"];
        var adminDisplayName = seedConfig["DisplayName"] ?? "System Administrator";

        // Skip if not configured (production should use different provisioning)
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Admin user seed skipped: SeedData:AdminUser:Email or Password not configured");
            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<PokokUser>>();
        var dbContext = serviceProvider.GetRequiredService<IdentityDbContext>();

        // Check for existing user without tenant filter to avoid duplicate key violation
        var existingAdmin = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == adminEmail.ToUpperInvariant());

        if (existingAdmin == null)
        {
            // System admin is created without a tenant (TenantId = null)
            // This allows cross-tenant management capabilities
            var adminUser = new PokokUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DisplayName = adminDisplayName,
                TenantId = null  // System-level admin, no tenant
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                logger.LogInformation("System admin user '{Email}' created (no tenant)", adminEmail);

                // Assign Admin role
                var roleResult = await userManager.AddToRoleAsync(adminUser, AdminRoleName);
                if (roleResult.Succeeded)
                    logger.LogInformation("Admin user assigned to '{RoleName}' role", AdminRoleName);
                else
                    logger.LogError("Failed to assign admin role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Ensure existing admin has the Admin role
            if (!await userManager.IsInRoleAsync(existingAdmin, AdminRoleName))
            {
                await userManager.AddToRoleAsync(existingAdmin, AdminRoleName);
                logger.LogInformation("Existing admin user assigned to '{RoleName}' role", AdminRoleName);
            }
        }
    }
}
