using System;
using System.Reflection;
using Eleven41.AspNetCore.MultiTenantIdentity;
using Eleven41.AspNetCore.MultiTenantIdentity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MultiTenantIdentityEntityFrameworkBuilderExtensions
    {
        public static MultiTenantIdentityBuilder AddMultiTenantEntityFrameworkStores<TContext>(this MultiTenantIdentityBuilder builder)
            where TContext : DbContext
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType, builder.TenantType, typeof(TContext));
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, Type tenantType, Type contextType)
        {
            var identityTenantType = FindGenericBaseType(tenantType, typeof(IdentityTenant<>));
            if (identityTenantType == null)
            {
                throw new InvalidOperationException("Tenant type not derived from IdentityTenant<>");
            }

            var keyType = identityTenantType.GenericTypeArguments[0];

            Type tenantStoreType = null;

            if (tenantStoreType == null)
            {
                var identityContext = FindGenericBaseType(contextType, typeof(MultiTenantIdentityDbContext<,,,>));
                if (identityContext != null)
                {
                    tenantStoreType = typeof(TenantStore<,,,>).MakeGenericType(userType, tenantType, contextType,
                        identityContext.GenericTypeArguments[3]);
                }
            }

            if (tenantStoreType == null)
            {
                var identityContext = FindGenericBaseType(contextType, typeof(MultiTenantIdentityUserContext<,,>));
                if (identityContext != null)
                {
                    tenantStoreType = typeof(TenantStore<,,,>).MakeGenericType(userType, tenantType, contextType,
                        identityContext.GenericTypeArguments[2]);
                }
            }

            if (tenantStoreType == null)
            {
                // If its a custom DbContext, we can only add the default POCOs
                tenantStoreType = typeof(TenantStore<,,,>).MakeGenericType(userType, tenantType, contextType, keyType);
            }

            services.TryAddScoped(typeof(ITenantStore<,>).MakeGenericType(userType, tenantType), tenantStoreType);
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType)
                {
                    return typeInfo;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
