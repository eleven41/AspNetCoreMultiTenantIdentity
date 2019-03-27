using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public interface ITenantValidator<TUser, TTenant>
        where TUser : class
        where TTenant : class
    {
        Task<IdentityResult> ValidateAsync(TenantManager<TUser, TTenant> manager, TTenant tenant);
    }
}