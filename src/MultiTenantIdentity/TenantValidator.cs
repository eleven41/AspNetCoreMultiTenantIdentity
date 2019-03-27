using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class TenantValidator<TUser, TTenant> : ITenantValidator<TUser, TTenant>
        where TUser : class
        where TTenant : class
    {
        public TenantValidator(MultiTenantIdentityErrorDescriber errors = null)
        {
            Describer = errors ?? new MultiTenantIdentityErrorDescriber();
        }

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to provider error messages for the current <see cref="UserValidator{TUser}"/>.
        /// </summary>
        /// <value>The <see cref="IdentityErrorDescriber"/> used to provider error messages for the current <see cref="UserValidator{TUser}"/>.</value>
        public MultiTenantIdentityErrorDescriber Describer { get; private set; }

        public async Task<IdentityResult> ValidateAsync(TenantManager<TUser, TTenant> manager, TTenant tenant)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            var errors = new List<IdentityError>();
            await ValidateDomain(manager, tenant, errors);
            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task ValidateDomain(TenantManager<TUser, TTenant> manager, TTenant tenant, List<IdentityError> errors)
        {
            var domain = await manager.GetDomainAsync(tenant);
            if (String.IsNullOrWhiteSpace(domain))
            {
                errors.Add(Describer.InvalidDomain(domain));
            }
            else if (!string.IsNullOrEmpty(manager.Options.Tenant.AllowedDomainCharacters) &&
                domain.Any(c => !manager.Options.Tenant.AllowedDomainCharacters.Contains(c)))
            {
                errors.Add(Describer.InvalidDomain(domain));
            }
            else
            {
                var owner = await manager.FindByDomainAsync(domain);
                if (owner != null &&
                    !String.Equals(await manager.GetTenantIdAsync(owner), await manager.GetTenantIdAsync(tenant)))
                {
                    errors.Add(Describer.DuplicateDomain(domain));
                }
            }
        }
    }
}