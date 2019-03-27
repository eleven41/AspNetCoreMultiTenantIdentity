using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore
{
    public class MultiTenantIdentityDbContext : MultiTenantIdentityDbContext<MultiTenantUser>
    {
        public MultiTenantIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityDbContext<TUser> : MultiTenantIdentityDbContext<TUser, IdentityTenant>
        where TUser : MultiTenantUser<string>
    {
        public MultiTenantIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityDbContext<TUser, TTenant> : MultiTenantIdentityDbContext<TUser, TTenant, MultiTenantRole>
        where TUser : MultiTenantUser<string>
        where TTenant : IdentityTenant<string>
    {
        public MultiTenantIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityDbContext<TUser, TTenant, TRole> : MultiTenantIdentityDbContext<TUser, TTenant, TRole, string>
        where TUser : MultiTenantUser<string>
        where TTenant : IdentityTenant<string>
        where TRole : MultiTenantRole<string>
    {
        public MultiTenantIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityDbContext<TUser, TTenant, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : MultiTenantUser<TKey>
        where TTenant : IdentityTenant<TKey>
        where TRole : MultiTenantRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public MultiTenantIdentityDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<TTenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            MultiTenantIdentityContextCommon<TUser, TTenant, TKey>.OnModelCreating(builder, this);
        }
    }
}
