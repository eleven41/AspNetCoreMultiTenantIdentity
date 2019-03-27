using System;
using System.Linq;
using System.Threading.Tasks;
using Eleven41.AspNetCore.MultiTenantIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MultiTenantIdentityBuilderExtensions
    {
        public static MultiTenantIdentityBuilder AddMultiTenantIdentity<TTenant>(this IdentityBuilder builder)
            where TTenant : class
            => builder.AddMultiTenantIdentity<TTenant>(_ => { });

        public static MultiTenantIdentityBuilder AddMultiTenantIdentity<TTenant>(
            this IdentityBuilder builder,
            Action<MultiTenantIdentityOptions> configureOptions)
            where TTenant : class
        {
            Type tenantType = typeof(TTenant);

            builder.Services.Configure<MultiTenantIdentityOptions>(o =>
            {
                // Set any defaults here

                // Pass through
                configureOptions?.Invoke(o);
            });

            {
                // Add our own tenant manager
                var concreteType = typeof(TenantManager<,>).MakeGenericType(builder.UserType, tenantType);
                builder.Services.TryAddScoped(concreteType);
            }

            {
                // Add our own tenant validator
                var concreteType = typeof(TenantValidator<,>).MakeGenericType(builder.UserType, tenantType);
                var baseType = typeof(ITenantValidator<,>).MakeGenericType(builder.UserType, tenantType);
                builder.Services.TryAddScoped(baseType, concreteType);
            }

            var serviceProvider = builder.Services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<MultiTenantIdentityOptions>>();

            if (options.Value.Users.IsUsersIsolated)
            {
                // Remove the built-in validator that checks for duplicate usernames
                // and use our own instead
                var baseType = typeof(IUserValidator<>).MakeGenericType(builder.UserType);
                var oldConcreteType = typeof(UserValidator<>).MakeGenericType(builder.UserType);

                var descriptors = builder.Services
                    .Where(d => d.ServiceType == baseType)
                    .Where(d => d.ImplementationType == oldConcreteType)
                    .ToList();
                foreach (var descriptor in descriptors)
                    builder.Services.Remove(descriptor);

                // Add our isolated user validator
                var newConcreteType = typeof(IsolatedUserValidator<,>).MakeGenericType(builder.UserType, builder.UserType.GenericTypeArguments[0]);
                builder.Services.AddScoped(baseType, newConcreteType);
            }

            return new MultiTenantIdentityBuilder(builder.UserType, tenantType, builder.RoleType, builder.Services);
        }
    }
}
