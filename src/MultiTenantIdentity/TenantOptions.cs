using System;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class TenantOptions
    {
        /// <summary>
        /// Gets or sets the list of allowed characters in the tenant domain used to validate tenants. Defaults to abcdefghijklmnopqrstuvwxyz0123456789-_
        /// </summary>
        /// <value>
        /// The list of allowed characters in the domain used to validate tenants.
        /// </value>
        public string AllowedDomainCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyz0123456789-_";
    }
}
