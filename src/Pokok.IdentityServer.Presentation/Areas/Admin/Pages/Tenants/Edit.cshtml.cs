using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using System.ComponentModel.DataAnnotations;

namespace Pokok.IdentityServer.Presentation.Areas.Admin.Pages.Tenants;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ITenantStore _tenantStore;

    public EditModel(ITenantStore tenantStore)
    {
        _tenantStore = tenantStore;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Subdomain can only contain lowercase letters, numbers, and hyphens")]
        public string? Subdomain { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var tenant = await _tenantStore.GetByIdAsync(id);
        if (tenant == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Subdomain = tenant.Subdomain,
            IsActive = tenant.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tenant = await _tenantStore.GetByIdAsync(Input.Id);
        if (tenant == null)
        {
            return NotFound();
        }

        tenant.Name = Input.Name;
        tenant.Subdomain = Input.Subdomain;
        tenant.IsActive = Input.IsActive;

        await _tenantStore.UpdateAsync(tenant);

        TempData["Message"] = $"Tenant '{tenant.Name}' updated successfully.";
        return RedirectToPage("Index");
    }
}
