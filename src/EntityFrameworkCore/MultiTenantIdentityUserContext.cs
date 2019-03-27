using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore
{
    public class MultiTenantIdentityUserContext : MultiTenantIdentityUserContext<MultiTenantUser>
    {
        public MultiTenantIdentityUserContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityUserContext<TUser> : MultiTenantIdentityUserContext<TUser, IdentityTenant>
        where TUser : MultiTenantUser<string>
    {
        public MultiTenantIdentityUserContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityUserContext<TUser, TTenant> : MultiTenantIdentityUserContext<TUser, TTenant, string>
        where TUser : MultiTenantUser<string>
        where TTenant : IdentityTenant<string>
    {
        public MultiTenantIdentityUserContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public class MultiTenantIdentityUserContext<TUser, TTenant, TKey> : IdentityUserContext<TUser, TKey>
        where TUser : MultiTenantUser<TKey>
        where TTenant : IdentityTenant<TKey>
        where TKey : IEquatable<TKey>
    {
        public MultiTenantIdentityUserContext(DbContextOptions options)
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
