using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class TenantManager<TUser, TTenant> : IDisposable
        where TUser : class
        where TTenant : class
    {
        public TenantManager(
            ITenantStore<TUser, TTenant> store,
            IOptions<MultiTenantIdentityOptions> optionsAccessor,
            IEnumerable<ITenantValidator<TUser, TTenant>> tenantValidators,
            ILookupNormalizer keyNormalizer,
            ILogger<TenantManager<TUser, TTenant>> logger
            )
        {
            this.Store = store ?? throw new ArgumentNullException(nameof(store));
            this.Options = optionsAccessor?.Value ?? new MultiTenantIdentityOptions();
            this.KeyNormalizer = keyNormalizer;
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (tenantValidators != null)
            {
                foreach (var v in tenantValidators)
                {
                    TenantValidators.Add(v);
                }
            }
        }

        protected ITenantStore<TUser, TTenant> Store { get; set; }
        public MultiTenantIdentityOptions Options { get; set; }
        public ILookupNormalizer KeyNormalizer { get; set; }
        public ILogger<TenantManager<TUser, TTenant>> Logger { get; set; }

        public IList<ITenantValidator<TUser, TTenant>> TenantValidators { get; } = new List<ITenantValidator<TUser, TTenant>>();

        /// <summary>
        /// The cancellation token used to cancel operations.
        /// </summary>
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public virtual bool SupportsQueryableTenants
        {
            get
            {
                ThrowIfDisposed();
                return this.Store is IQueryableTenantStore<TUser, TTenant>;
            }
        }

        /// <summary>
        /// Returns an IQueryable of tenants if the store is an IQueryableTenantStore
        /// </summary>
        public virtual IQueryable<TTenant> Tenants
        {
            get
            {
                var queryableStore = this.Store as IQueryableTenantStore<TUser, TTenant>;
                if (queryableStore == null)
                {
                    throw new NotSupportedException("Store is not IQueryable TenantStore");
                }
                return queryableStore.Tenants;
            }
        }

        /// <summary>
        /// Releases all resources used by the tenant manager.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                this.Store.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }


        /// <summary>
        /// Normalize domain for consistent comparisons.
        /// </summary>
        /// <param name="domain">The name to normalize.</param>
        /// <returns>A normalized value representing the specified <paramref name="domain"/>.</returns>
        public virtual string NormalizeDomain(string domain)
            => (KeyNormalizer == null) ? domain : KeyNormalizer.Normalize(domain);

        private async Task UpdateNormalizedDomainAsync(TTenant tenant)
        {
            var normalizedDomain = NormalizeDomain(await GetDomainAsync(tenant));
            //normalizedName = ProtectPersonalData(normalizedName);
            await this.Store.SetNormalizedDomainAsync(tenant, normalizedDomain, CancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            //await UpdateSecurityStampInternal(tenant);
            var result = await ValidateTenantAsync(tenant);
            if (!result.Succeeded)
            {
                return result;
            }

            await UpdateNormalizedDomainAsync(tenant);

            return await this.Store.CreateAsync(tenant, CancellationToken);
        }

        private async Task<IdentityResult> ValidateTenantAsync(TTenant tenant)
        {
            var errors = new List<IdentityError>();
            foreach (var v in this.TenantValidators)
            {
                var result = await v.ValidateAsync(this, tenant);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }
            if (errors.Count() > 0)
            {
                Logger.LogWarning(13, "Tenant {userId} validation failed: {errors}.", await GetTenantIdAsync(tenant), string.Join(";", errors.Select(e => e.Code)));
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return this.Store.DeleteAsync(tenant, CancellationToken);
        }

        public virtual Task<TTenant> FindByIdAsync(string tenantId)
        {
            ThrowIfDisposed();

            return this.Store.FindByIdAsync(tenantId, CancellationToken);
        }

        public Task<TTenant> FindByDomainAsync(string domain)
        {
            ThrowIfDisposed();

            return this.Store.FindByDomainAsync(domain, CancellationToken);
        }

        public Task<string> GetTenantIdAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            return this.Store.GetTenantIdAsync(tenant, CancellationToken);
        }

        public virtual Task<string> GetNameAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            return this.Store.GetNameAsync(tenant, CancellationToken);
        }

        public virtual Task<string> GetDomainAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            return this.Store.GetDomainAsync(tenant, CancellationToken);
        }

        public Task<IQueryable<TUser>> GetTenantUsersAsync(TTenant tenant)
        {
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            return this.Store.GetTenantUsersAsync(tenant, CancellationToken);
        }
    }
}
