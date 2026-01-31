using Pokok.Messaging.Email;

namespace Pokok.IdentityServer.Application.Options
{
    /// <summary>
    /// Configuration for all email templates used in IdentityServer.
    /// Maps template keys to their Subject/Body templates.
    /// </summary>
    public class EmailTemplatesConfig
    {
        public const string SectionName = "EmailTemplates";

        public Dictionary<string, EmailTemplateEntry> Templates { get; set; } = new();

        public EmailTemplateOptions? GetTemplate(string key)
        {
            return Templates.TryGetValue(key, out var template) ? template : null;
        }
    }

    /// <summary>
    /// Email template entry with Subject and Body
    /// </summary>
    public class EmailTemplateEntry : EmailTemplateOptions
    {
        public override string Subject { get; set; } = string.Empty;
        public override string Body { get; set; } = string.Empty;
    }

    /// <summary>
    /// Well-known email template keys used in IdentityServer
    /// </summary>
    public static class EmailTemplateKeys
    {
        public const string UserRegistrationConfirmation = "UserRegistrationConfirmation";
        public const string PasswordReset = "PasswordReset";
    }
}
