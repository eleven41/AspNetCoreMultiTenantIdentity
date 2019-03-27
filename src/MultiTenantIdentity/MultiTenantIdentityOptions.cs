using System;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class MultiTenantIdentityOptions
    {
        public TenantOptions Tenant { get; set; } = new TenantOptions();

        public UserOptions Users { get; set; } = new UserOptions();

        public StoreOptions Stores { get; set; } = new StoreOptions();
    }
}
