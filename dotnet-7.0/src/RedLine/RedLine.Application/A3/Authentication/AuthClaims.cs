namespace RedLine.A3.Authentication
{
    /// <summary>
    /// Defines the claims required for authentication.
    /// </summary>
    public static class AuthClaims
    {
        /// <summary>
        /// A claim for the user's email, which is also their login.
        /// </summary>
        public static readonly string Email = nameof(Email).ToLowerInvariant();

        /// <summary>
        /// A claim for the user's family name, or last name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Name matches claim name.")]
        public static readonly string Family_Name = nameof(Family_Name).ToLowerInvariant();

        /// <summary>
        /// A claim for the user's given name, or first name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Name matches claim name.")]
        public static readonly string Given_Name = nameof(Given_Name).ToLowerInvariant();

        /// <summary>
        /// A claim to get the user's display name.
        /// </summary>
        public static readonly string Name = nameof(Name).ToLowerInvariant();

        /// <summary>
        /// A claim for the immutable user principle name (UPN), which is a unique value for the user within the identity provider.
        /// </summary>
        public static readonly string Upn = nameof(Upn).ToLowerInvariant();
    }
}
