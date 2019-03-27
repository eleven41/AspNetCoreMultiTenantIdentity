using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public interface ITenantStore<TUser, TTenant> : IDisposable
        where TUser : class
        where TTenant : class
    {
        // CRUD
        Task<IdentityResult> CreateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> UpdateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> DeleteAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));

        // Find
        Task<TTenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default(CancellationToken));
        Task<TTenant> FindByDomainAsync(string normalizedDomain, CancellationToken cancellationToken = default(CancellationToken));

        // Get tenant properties
        Task<string> GetTenantIdAsync(TTenant tenant, CancellationToken cancellationToken);
        Task<string> GetNameAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetDomainAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> GetIsSoftSuspendedAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> GetIsHardSuspendedAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));

        // Set tenant properties
        Task SetNormalizedDomainAsync(TTenant tenant, string normalizedDomain, CancellationToken cancellationToken = default(CancellationToken));

        // Additional lookups
        Task<IQueryable<TUser>> GetTenantUsersAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));

    }
}
