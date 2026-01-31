namespace Pokok.IdentityServer.Domain.Entities;

/// <summary>
/// Represents a generic tenant in the identity system.
/// NOT tied to any specific business domain (Property, JMB, etc.)
/// </summary>
public class Tenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Name { get; set; } = string.Empty;
    
    public string? Subdomain { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Optional metadata for tenant-specific configuration
    /// </summary>
    public string? Metadata { get; set; }
}
