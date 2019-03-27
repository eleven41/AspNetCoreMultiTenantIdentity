using System;
using System.Reflection;
using Eleven41.AspNetCore.MultiTenantIdentity;

namespace Microsoft.Extensions.DependencyInjection
{
    public class MultiTenantIdentityBuilder
    {
        public MultiTenantIdentityBuilder(Type user, Type tenant, IServiceCollection services)
        {
            this.UserType = user;
            this.TenantType = tenant;
            this.Services = services;
        }

        public MultiTenantIdentityBuilder(Type user, Type tenant, Type role, IServiceCollection services)
        {
            this.UserType = user;
            this.TenantType = tenant;
            this.RoleType = role;
            this.Services = services;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> used for users.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for users.
        /// </value>
        public Type UserType { get; private set; }


        /// <summary>
        /// Gets the <see cref="Type"/> used for roles.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for roles.
        /// </value>
        public Type RoleType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> used for tenants.
        /// </summary>
        /// <value>
        /// The <see cref="Type"/> used for tenants.
        /// </value>
        public Type TenantType { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        public IServiceCollection Services { get; private set; }

        private MultiTenantIdentityBuilder AddScoped(Type serviceType, Type concreteType)
        {
            this.Services.AddScoped(serviceType, concreteType);
            return this;
        }

        public virtual MultiTenantIdentityBuilder AddTenantValidator<TValidator>()
            where TValidator : class
        {
            return AddScoped(typeof(ITenantValidator<,>).MakeGenericType(this.UserType, this.TenantType), typeof(TValidator));
        }

        public virtual MultiTenantIdentityBuilder AddErrorDescriber<TDescriber>() 
            where TDescriber : MultiTenantIdentityErrorDescriber
        {
            this.Services.AddScoped<MultiTenantIdentityErrorDescriber, TDescriber>();
            return this;
        }

        public virtual MultiTenantIdentityBuilder AddTenantStore<TTenantStore>() 
            where TTenantStore : class
        {
            var storeType = typeof(ITenantStore<,>).MakeGenericType(this.UserType, this.TenantType);
            this.Services.AddScoped(storeType, typeof(TTenantStore));
            return this;
        }

        public MultiTenantIdentityBuilder AddTenantManager<TTenantManager>() 
            where TTenantManager : class
        {
            var managerType = typeof(TenantManager<,>).MakeGenericType(this.UserType, this.TenantType);
            Type customType = typeof(TTenantManager);
            if (!managerType.GetTypeInfo().IsAssignableFrom(customType.GetTypeInfo()))
            {
                throw new InvalidOperationException(String.Format("Type {0} must derive from {1}<{2},{3}>.", customType.Name, "TenantManager", this.UserType.Name, this.TenantType.Name));
            }
            if (managerType != customType)
            {
                this.Services.AddScoped(customType, services => services.GetRequiredService(managerType));
            }
            this.Services.AddScoped(managerType, customType);
            return this;
        }
    }
}
