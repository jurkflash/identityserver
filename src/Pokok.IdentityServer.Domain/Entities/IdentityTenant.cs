using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokok.IdentityServer.Domain.Entities
{
    public class IdentityTenant
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        private IdentityTenant() { } // EF Core

        public IdentityTenant(string name, string description)
        {
            Name = name;
            Description = description;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
