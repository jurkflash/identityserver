using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pokok.IdentityServer.Infrastructure.DuendeIdentityServer
{
    public class PokokProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var tenantId = context.Subject.FindFirst("tenant_id")?.Value;

            if (!string.IsNullOrEmpty(tenantId))
            {
                context.IssuedClaims.Add(
                    new Claim("tenant_id", tenantId)
                );
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
            => Task.CompletedTask;
    }
}
