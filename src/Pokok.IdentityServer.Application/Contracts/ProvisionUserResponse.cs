namespace Pokok.IdentityServer.Application.Contracts
{
    public class ProvisionUserResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? PasswordResetLink { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
