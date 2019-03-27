using System;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class MultiTenantRole : MultiTenantRole<string> { }

    public class MultiTenantRole<TKey> : IdentityRole<TKey> 
        where TKey : IEquatable<TKey>
    {
    }
}