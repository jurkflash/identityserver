using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pokok.BuildingBlocks.Outbox;
using Pokok.IdentityServer.Infrastructure.Identity;

namespace Pokok.IdentityServer.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("IdentityConnection"))); // or UseSqlServer

            // Register ASP.NET Identity with your custom PokokUser
            services.AddIdentity<PokokUser, PokokRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                // Add more identity options here
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(ServiceCollectionExtensions).Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("IdentityConnection");

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
            });

            return services;
        }

        public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

            return services;
        }
    }
}
