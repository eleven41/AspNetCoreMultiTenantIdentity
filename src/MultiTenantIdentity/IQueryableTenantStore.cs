using System;
using System.Linq;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public interface IQueryableTenantStore<TUser, TTenant> : ITenantStore<TUser, TTenant>
        where TUser : class
        where TTenant : class
    {
        IQueryable<TTenant> Tenants { get; }
    }
}
