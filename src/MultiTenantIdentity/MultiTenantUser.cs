using System;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class MultiTenantUser : MultiTenantUser<string> { }

    public class MultiTenantUser<TKey> : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        public TKey TenantId { get; set; }
    }
}
