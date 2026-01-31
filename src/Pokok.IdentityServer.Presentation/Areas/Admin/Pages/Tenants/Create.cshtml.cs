using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokok.IdentityServer.Application.Contracts.Persistence;
using Pokok.IdentityServer.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Pokok.IdentityServer.Presentation.Areas.Admin.Pages.Tenants;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ITenantStore _tenantStore;

    public CreateModel(ITenantStore tenantStore)
    {
        _tenantStore = tenantStore;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Subdomain can only contain lowercase letters, numbers, and hyphens")]
        public string? Subdomain { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid().ToString(),
            Name = Input.Name,
            Subdomain = Input.Subdomain,
            IsActive = Input.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _tenantStore.CreateAsync(tenant);

        TempData["Message"] = $"Tenant '{tenant.Name}' created successfully.";
        return RedirectToPage("Index");
    }
}
