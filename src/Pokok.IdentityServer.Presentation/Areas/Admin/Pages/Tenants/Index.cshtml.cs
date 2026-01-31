using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Domain.Entities;

namespace Pokok.IdentityServer.Presentation.Areas.Admin.Pages.Tenants;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ITenantStore _tenantStore;

    public IndexModel(ITenantStore tenantStore)
    {
        _tenantStore = tenantStore;
    }

    public IEnumerable<Tenant> Tenants { get; set; } = new List<Tenant>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        Tenants = await _tenantStore.GetAllActiveAsync();
    }
}
