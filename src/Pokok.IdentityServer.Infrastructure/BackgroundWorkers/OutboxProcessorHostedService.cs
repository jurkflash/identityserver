using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokok.IdentityServer.Infrastructure.Outbox;
using Pokok.BuildingBlocks.Messaging.Abstractions;

namespace Pokok.IdentityServer.Infrastructure.BackgroundWorkers
{
    public class OutboxProcessorHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorHostedService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

        public OutboxProcessorHostedService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Processor started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServerOutboxDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedOnUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(10)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            await publisher.PublishAsync(message.Type.Value, message.Payload, stoppingToken);
                            message.MarkAsProcessed();
                        }
                        catch (Exception ex)
                        {
                            message.MarkAsFailed(ex.Message);
                            _logger.LogError(ex, "Failed to process outbox message {Id}", message.Id);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in OutboxProcessorHostedService");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
