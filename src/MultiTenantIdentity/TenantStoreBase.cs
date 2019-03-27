using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public abstract class TenantStoreBase<TUser, TTenant, TKey> : 
        IQueryableTenantStore<TUser, TTenant>
        where TUser : class
        where TTenant : IdentityTenant<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenantStoreBase(IdentityErrorDescriber describer)
        {
            this.ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        public IdentityErrorDescriber ErrorDescriber { get; private set; }

        /// <summary>
        /// Dispose the store
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
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TKey id)
        {
            if (object.Equals(id, default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }

        public Task<string> GetTenantIdAsync(TTenant tenant, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(ConvertIdToString(tenant.Id));
        }

        public virtual Task<string> GetNameAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.Name);
        }

        public virtual Task<string> GetDomainAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.Domain);
        }

        public virtual Task<bool> GetIsSoftSuspendedAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.IsSoftSuspended);
        }

        public virtual Task<bool> GetIsHardSuspendedAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.IsHardSuspended);
        }

        public virtual Task SetNormalizedDomainAsync(TTenant tenant, string normalizedDomain, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            tenant.NormalizedDomain = normalizedDomain;
            return Task.CompletedTask;
        }

        public abstract IQueryable<TTenant> Tenants
        {
            get;
        }

        public abstract Task<IdentityResult> CreateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        public abstract Task<IdentityResult> UpdateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));
        public abstract Task<IdentityResult> DeleteAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task<TTenant> FindByDomainAsync(string normalizedDomain, CancellationToken cancellationToken = default(CancellationToken));
        public abstract Task<TTenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task<IQueryable<TUser>> GetTenantUsersAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken));


    }
}
