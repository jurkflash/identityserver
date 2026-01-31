using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokok.IdentityServer.Application.Contracts.Identity;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pokok.IdentityServer.Presentation.Areas.Identity.Pages.Account;

public class SelectTenantModel : PageModel
{
    private readonly ITenantStore _tenantStore;
    private readonly ITenantContext _tenantContext;

    public SelectTenantModel(ITenantStore tenantStore, ITenantContext tenantContext)
    {
        _tenantStore = tenantStore;
        _tenantContext = tenantContext;
    }

    public IEnumerable<Tenant> Tenants { get; set; } = new List<Tenant>();

    [BindProperty]
    [Required(ErrorMessage = "Please select an organization")]
    public string SelectedTenantId { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Tenants = await _tenantStore.GetAllActiveAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Tenants = await _tenantStore.GetAllActiveAsync();
            return Page();
        }

        var isValid = await _tenantStore.IsValidAsync(SelectedTenantId);
        if (!isValid)
        {
            ErrorMessage = "Invalid organization selected";
            Tenants = await _tenantStore.GetAllActiveAsync();
            return Page();
        }

        // Set tenant in cookie
        Response.Cookies.Append("TenantId", SelectedTenantId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        // Set in context
        _tenantContext.SetTenant(SelectedTenantId);

        return LocalRedirect(ReturnUrl ?? "~/");
    }
}
