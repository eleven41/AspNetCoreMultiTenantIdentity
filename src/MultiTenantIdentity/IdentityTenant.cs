using System;

namespace Eleven41.AspNetCore.MultiTenantIdentity
{
    public class IdentityTenant : IdentityTenant<string> { }

    public class IdentityTenant<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        /// <summary>
        /// Optional name of the organization
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional unique domain/slug/id for the organization
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Normalized version of the domain
        /// </summary>
        public string NormalizedDomain { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Users can sign-in, but functionality should be limited
        /// </summary>
        public bool IsSoftSuspended { get; set; }

        /// <summary>
        /// Users should not be able to sign-in
        /// </summary>
        public bool IsHardSuspended { get; set; }
    }
}
