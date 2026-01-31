using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pokok.BuildingBlocks.Messaging.Abstractions;
using Pokok.BuildingBlocks.Messaging.RabbitMQ;
using Pokok.BuildingBlocks.Outbox;
using Pokok.IdentityServer.Application.Contracts.Identity;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Infrastructure.DuendeIdentityServer;
using Pokok.IdentityServer.Infrastructure.Identity;
using Pokok.IdentityServer.Infrastructure.Outbox;
using Pokok.IdentityServer.Infrastructure.Persistence;
using System.Runtime;

namespace Pokok.IdentityServer.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection"))); // or UseSqlServer

            // Register HttpContextAccessor for tenant resolution
            services.AddHttpContextAccessor();

            // Register multi-tenancy services
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver, CompositeTenantResolver>();
            services.AddScoped<ITenantStore, TenantStore>();

            // Register ASP.NET Identity with your custom PokokUser and custom claims factory
            services.AddIdentity<PokokUser, PokokRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                // Add more identity options here
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<PokokUserClaimsPrincipalFactory>();

            return services;
        }

        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("IdentityConnection");

            // Register IdentityServer options
            services.Configure<IdentityServerOptions>(configuration.GetSection(IdentityServerOptions.SectionName));

            // IdentityServer with EF-based config + operational store
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddAspNetIdentity<PokokUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));

                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddProfileService<PokokProfileService>();

            return services;
        }

        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityServerOutboxDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityConnection"))); 
            services.AddScoped<OutboxDbContext>(sp => sp.GetRequiredService<IdentityServerOutboxDbContext>());
            services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
            services.AddOutboxProcessor<IdentityServerOutboxDbContext>();

            return services;
        }
    }
}
