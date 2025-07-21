using Pokok.BuildingBlocks.Cqrs.Validation;

namespace Pokok.IdentityServer.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public void Validate(RegisterUserCommand request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!IsValidEmail(request.Email))
                errors.Add("Email format is invalid.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");
            else if (request.Password.Length < 6)
                errors.Add("Password must be at least 6 characters.");

            if (!string.IsNullOrWhiteSpace(request.DisplayName) && request.DisplayName.Length > 100)
                errors.Add("Display name cannot exceed 100 characters.");

            if (errors.Any())
                throw new ValidationException(errors);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
