using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Application.Contracts;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Application.Extensions;
using Pokok.IdentityServer.Infrastructure.Identity;
using System.Text;

namespace Pokok.IdentityServer.Presentation.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<PokokUser> _userManager;
        private readonly IUserStore<PokokUser> _userStore;
        private readonly IUserEmailStore<PokokUser> _emailStore;
        private readonly ILogger<UsersController> _logger;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ITenantStore _tenantStore;

        public UsersController(
            UserManager<PokokUser> userManager,
            IUserStore<PokokUser> userStore,
            ILogger<UsersController> logger,
            IDomainEventDispatcher dispatcher,
            ITenantStore tenantStore)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _dispatcher = dispatcher;
            _tenantStore = tenantStore;
        }

        /// <summary>
        /// Creates a new user without a password. User will need to set password via reset link.
        /// Requires authentication with a client that has tenant_id claim configured.
        /// The user will be assigned to the tenant associated with the calling client.
        /// </summary>
        /// <param name="request">User creation request</param>
        /// <returns>User creation response with password reset link</returns>
        [HttpPost]
        public async Task<ActionResult<ProvisionUserResponse>> CreateUser([FromBody] ProvisionUserRequest request)
        {
            // Extract tenant_id from the client credentials token
            var tenantId = User.GetTenantId();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                _logger.LogWarning("User creation attempted without tenant_id claim. ClientId: {ClientId}", 
                    User.GetClientId());
                return Unauthorized(new ProvisionUserResponse
                {
                    Success = false,
                    Errors = new List<string> 
                    { 
                        "Client is not authorized to create users. Missing tenant_id claim." 
                    }
                });
            }

            // Validate the tenant exists and is active
            var isValidTenant = await _tenantStore.IsValidAsync(tenantId);
            if (!isValidTenant)
            {
                _logger.LogWarning("User creation attempted for invalid/inactive tenant: {TenantId}", tenantId);
                return BadRequest(new ProvisionUserResponse
                {
                    Success = false,
                    Errors = new List<string> 
                    { 
                        $"Tenant '{tenantId}' is not valid or is inactive." 
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ProvisionUserResponse
                {
                    Success = false,
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning(
                    "User creation attempted with existing email: {Email}, TenantId: {TenantId}", 
                    request.Email, tenantId);
                
                return Conflict(new ProvisionUserResponse
                {
                    Success = false,
                    Errors = [$"A user with email '{request.Email}' already exists."]
                });
            }

            var user = new PokokUser()
            {
                DisplayName = request.DisplayName,
                TenantId = tenantId  // Assign user to the client's tenant
            };

            await _userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);

            // Create user without password
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ProvisionUserResponse
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            _logger.LogInformation(
                "User created without password via API. Email: {Email}, TenantId: {TenantId}, ClientId: {ClientId}", 
                request.Email, tenantId, User.GetClientId());

            var userId = await _userManager.GetUserIdAsync(user);
            
            // Generate password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
            
            // Create password reset callback URL
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = encodedToken },
                protocol: Request.Scheme);

            // Raise domain event for user registration
            var domainUser = Domain.Aggregates.Users.User.RegisterWithoutPassword(
                new UserId(Guid.Parse(userId)),
                new Email(request.Email),
                new DisplayName(request.DisplayName));

            await _dispatcher.DispatchAsync(domainUser.DomainEvents);
            domainUser.ClearDomainEvents();

            return Ok(new ProvisionUserResponse
            {
                Success = true,
                UserId = userId,
                Email = request.Email,
                DisplayName = request.DisplayName,
                TenantId = tenantId,
                PasswordResetLink = callbackUrl
            });
        }

        private IUserEmailStore<PokokUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<PokokUser>)_userStore;
        }
    }
}
