using Microsoft.Extensions.Options;
using Pokok.BuildingBlocks.Outbox;
using Pokok.IdentityServer.Application.Options;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.Messaging.Email;

namespace Pokok.IdentityServer.Application.DomainEventHandlers
{
    public class PasswordResetRequestedHandler 
        : EmailDomainEventHandler<PasswordResetRequested>
    {
        private readonly EmailTemplatesConfig _config;

        public PasswordResetRequestedHandler(
            ITemplateRenderer renderer,
            IOutboxMessageRepository outboxMessageRepository,
            IOptions<EmailTemplatesConfig> config)
            : base(renderer, outboxMessageRepository)
        {
            _config = config.Value;
        }

        protected override EmailData GetEmailData(PasswordResetRequested domainEvent)
        {
            var template = _config.GetTemplate(EmailTemplateKeys.PasswordReset)
                ?? throw new InvalidOperationException($"Email template '{EmailTemplateKeys.PasswordReset}' not configured");

            return new EmailData(
                TemplateOptions: template,
                TemplateData: new
                {
                    DisplayName = domainEvent.DisplayName.Value,
                    CallbackUrl = domainEvent.ResetLink.Value
                },
                Recipients: [domainEvent.Email.Value],
                OccurredOn: domainEvent.OccurredOn);
        }
    }
}
