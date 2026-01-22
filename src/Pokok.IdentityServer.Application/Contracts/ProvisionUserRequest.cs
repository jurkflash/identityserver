using System.ComponentModel.DataAnnotations;

namespace Pokok.IdentityServer.Application.Contracts
{
    public class ProvisionUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty;

        public Guid? TenantId { get; set; }
    }
}
