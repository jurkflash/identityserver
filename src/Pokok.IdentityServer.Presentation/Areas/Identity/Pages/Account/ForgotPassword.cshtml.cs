// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Pokok.IdentityServer.Domain.Aggregates.Users.Events;
using Pokok.IdentityServer.Domain.ValueObjects;
using Pokok.IdentityServer.Infrastructure.Identity;

namespace Pokok.IdentityServer.Presentation.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<PokokUser> _userManager;
        private readonly IDomainEventDispatcher _dispatcher;

        public ForgotPasswordModel(UserManager<PokokUser> userManager, IDomainEventDispatcher dispatcher)
        {
            _userManager = userManager;
            _dispatcher = dispatcher;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                var passwordResetEvent = new PasswordResetRequested(
                    new UserId(Guid.Parse(user.Id)),
                    new Email(Input.Email),
                    new DisplayName(user.DisplayName ?? user.UserName),
                    new ConfirmationLink(callbackUrl));

                await _dispatcher.DispatchAsync(new[] { passwordResetEvent });

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
