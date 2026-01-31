using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Pokok.BuildingBlocks.Outbox;
using Pokok.Messaging.Email;
using System.Text.Json;

namespace Pokok.IdentityServer.Application.DomainEventHandlers
{
    /// <summary>
    /// Base handler for domain events that send emails via the outbox pattern.
    /// Reduces duplication across email-sending domain event handlers.
    /// </summary>
    public abstract class EmailDomainEventHandler<TEvent> : IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        private readonly ITemplateRenderer _renderer;
        private readonly IOutboxMessageRepository _outboxMessageRepository;

        protected EmailDomainEventHandler(
            ITemplateRenderer renderer,
            IOutboxMessageRepository outboxMessageRepository)
        {
            _renderer = renderer;
            _outboxMessageRepository = outboxMessageRepository;
        }

        public async Task Handle(TEvent domainEvent, CancellationToken cancellationToken)
        {
            var emailData = GetEmailData(domainEvent);
            
            var template = _renderer.Render(emailData.TemplateOptions, emailData.TemplateData);

            var emailPayload = new EmailDispatchMessage
            {
                To = emailData.Recipients,
                Subject = template.Subject,
                Body = template.Body,
            };

            var outboxMessage = new OutboxMessage(
                OutboxMessageType.Email,
                JsonSerializer.Serialize(emailPayload),
                "IdentityServer",
                emailData.OccurredOn);

            await _outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);
            await _outboxMessageRepository.CompleteAsync();
        }

        /// <summary>
        /// Extract email data from the domain event.
        /// Implement this in derived classes to provide event-specific data.
        /// </summary>
        protected abstract EmailData GetEmailData(TEvent domainEvent);

        protected record EmailData(
            EmailTemplateOptions TemplateOptions,
            object TemplateData,
            List<string> Recipients,
            DateTime OccurredOn);
    }
}
