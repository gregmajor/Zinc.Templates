namespace RedLine.A3.Authorization.Domain
{
    /// <summary>
    /// Defines the most common grant types, but it's not an exhaustive list.
    /// </summary>
    /// <remarks>
    /// In general, the grant type can be anything that makes sense for a given application.
    /// </remarks>
    public static class GrantType
    {
        /// <summary>
        /// The Activity grant type is an "explicit" grant that bestows execute rights for an activity (command or query).
        /// </summary>
        public static readonly string Activity = nameof(Activity);

        /// <summary>
        /// The ActivityGroup grant type is an "implicit" grant that bestows execute rights for all the activities (commands or queries) in the group.
        /// </summary>
        public static readonly string ActivityGroup = nameof(ActivityGroup);
    }
}
