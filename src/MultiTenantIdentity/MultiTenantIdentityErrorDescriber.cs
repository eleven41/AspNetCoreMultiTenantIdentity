using System;
using Microsoft.AspNetCore.Identity;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class MultiTenantIdentityErrorDescriber
    {
        public virtual IdentityError InvalidDomain(string domain)
        {
            return new IdentityError
            {
                Code = nameof(InvalidDomain),
                Description = $"Invalid domain '{domain}'",
            };
        }

        public virtual IdentityError DuplicateDomain(string domain)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateDomain),
                Description = $"Domain '{domain}' is already taken",
            };
        }
    }
}