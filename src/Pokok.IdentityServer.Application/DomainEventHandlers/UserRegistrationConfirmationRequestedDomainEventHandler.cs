using Pokok.BuildingBlocks.Domain.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;
using Pokok.BuildingBlocks.Outbox;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.Messaging.Email;
using System.Text.Json;

namespace Pokok.IdentityServer.Application.DomainEventHandlers
{
    public class UserRegistrationConfirmationRequestedDomainEventHandler : IDomainEventHandler<UserRegistrationConfirmationRequestedDomainEvent>
    {
        private readonly ITemplateRenderer _renderer;
        private readonly IOutboxMessageRepository _outboxMessageRepository;

        public UserRegistrationConfirmationRequestedDomainEventHandler(ITemplateRenderer renderer, IOutboxMessageRepository outboxMessageRepository)
        {
            _renderer = renderer;
            _outboxMessageRepository = outboxMessageRepository;
        }

        public async Task Handle(UserRegistrationConfirmationRequestedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var template = _renderer.Render(EmailTemplateKey.UserRegisteredConfirmation, new
            {
                DisplayName = domainEvent.DisplayName.Value,
                CallbackUrl = domainEvent.ConfirmationLink
            });

            var emailPayload = new EmailDispatchMessage
            {
                To = new List<string> { domainEvent.Email.Value },
                Subject = template.Subject,
                Body = template.Body,
                TemplateKey = EmailTemplateKey.UserRegisteredConfirmation.ToString(),
            };

            var outboxMessage = new OutboxMessage(OutboxMessageType.Email, 
                                                  JsonSerializer.Serialize(emailPayload), 
                                                  "IdentityServer", 
                                                  domainEvent.OccurredOn);

            await _outboxMessageRepository.AddAsync(outboxMessage, cancellationToken);
            await _outboxMessageRepository.CompleteAsync();
        }
    }
}
