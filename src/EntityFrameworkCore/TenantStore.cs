using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore
{
    public class TenantStore<TUser, TTenant, TContext> : TenantStore<TUser, TTenant, TContext, string>
        where TUser : MultiTenantUser<string>
        where TTenant : IdentityTenant<string>
        where TContext : DbContext
    {
        public TenantStore(TContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }
    }

    public class TenantStore<TUser, TTenant, TContext, TKey> : TenantStoreBase<TUser, TTenant, TKey>
        where TUser : MultiTenantUser<TKey>
        where TTenant : IdentityTenant<TKey>
        where TContext : DbContext
        where TKey : IEquatable<TKey>
    {
        public TenantStore(TContext context, IdentityErrorDescriber describer = null)
            : base(describer)
        {
            this.Context = context;
        }

        public TContext Context { get; private set; }

        private DbSet<TTenant> TenantsSet { get { return Context.Set<TTenant>(); } }

        /// <summary>
        /// A navigation property for the tenants the store contains.
        /// </summary>
        public override IQueryable<TTenant> Tenants
        {
            get { return TenantsSet; }
        }

        private DbSet<TUser> UsersSet { get { return Context.Set<TUser>(); } }

        /// <summary>
        /// A navigation property for the tenants the store contains.
        /// </summary>
        public IQueryable<TUser> Users
        {
            get { return UsersSet; }
        }

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
        }

        public override async Task<IdentityResult> CreateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            Context.Add(tenant);
            await SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            Context.Attach(tenant);
            tenant.ConcurrencyStamp = Guid.NewGuid().ToString();
            Context.Update(tenant);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> DeleteAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            Context.Remove(tenant);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public override Task<TTenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var id = ConvertIdFromString(tenantId);
            return this.TenantsSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public override Task<TTenant> FindByDomainAsync(string normalizedDomain, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return this.Tenants
                .FirstOrDefaultAsync(u => u.NormalizedDomain == normalizedDomain, cancellationToken);
        }

        public override Task<IQueryable<TUser>> GetTenantUsersAsync(TTenant tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Task.FromResult(this.Users
                .Where(u => u.TenantId.Equals(tenant.Id)));
        }
    }
}
