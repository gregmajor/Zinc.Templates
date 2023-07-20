using RedLine.Domain;

namespace RedLine.A3.Authorization
{
    /// <summary>
    /// The cache keys used for authorization.
    /// </summary>
    public static class AuthorizationCacheKey
    {
        /// <summary>
        /// Get the key used for a user's grants.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The formatted key.</returns>
        public static string ForGrants(string userId) => $"{ApplicationContext.Get(nameof(AuthorizationCacheKey))}/authorization/{userId}/grants";

        /// <summary>
        /// Get the key used for activity groups.
        /// </summary>
        /// <returns>The formatted key.</returns>
        public static string ForActivityGroups() => $"{ApplicationContext.Get(nameof(AuthorizationCacheKey))}/authorization/activity-groups";
    }
}
