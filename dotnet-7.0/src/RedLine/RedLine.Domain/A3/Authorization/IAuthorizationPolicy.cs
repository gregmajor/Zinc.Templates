using RedLine.Domain.Policies;

namespace RedLine.A3.Authorization
{
    /// <summary>
    /// A policy which applies authorization.
    /// </summary>
    public interface IAuthorizationPolicy : IPolicy
    {
        /// <summary>
        /// Gets tenant for which the authorization policy applies.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Gets the id of the user for whom the authorization policy applies.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Determines if the user is authorized for the activity. If the activity pertains to a resource, that is checked as well.
        /// </summary>
        /// <param name="activityName">The name of the activity being performed.</param>
        /// <param name="resourceId">The key of a resource related to the activity, if any.</param>
        /// <returns>True if the user is authorized to perform the activity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "It is a valid optional parameter.")]
        bool IsAuthorized(string activityName, string resourceId = null);

        /// <summary>
        /// Determines if the user has the activity grant, explicitly or implicitly.
        /// </summary>
        /// <param name="activityName">The name of the activity to perform.</param>
        /// <returns>True if the user has a grant which matches the activity.</returns>
        bool HasGrant(string activityName);

        /// <summary>
        /// Determines if the user has the activity grant, explicitly or implicitly.
        /// </summary>
        /// <typeparam name="TActivity">The type of activity to perform.</typeparam>
        /// <returns>True if the user has a grant which matches the activity.</returns>
        bool HasGrant<TActivity>();

        /// <summary>
        /// Determines if the user has the resource grant, explicitly or implicitly.
        /// </summary>
        /// <param name="resourceType">The type of the resource.</param>
        /// <param name="resourceId">The resource identifier.</param>
        /// <returns>True if the user has a grant which matches the resource.</returns>
        bool HasGrant(string resourceType, string resourceId);

        /// <summary>
        /// Determines if the user has the resource grant, explicitly or implicitly.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <typeparam name="TResourceType">The type of the resource.</typeparam>
        /// <returns>True if the user has a grant which matches the resource.</returns>
        bool HasGrant<TResourceType>(string resourceId);
    }
}
