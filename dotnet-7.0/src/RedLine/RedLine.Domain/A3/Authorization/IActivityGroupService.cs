using System.Threading.Tasks;

namespace RedLine.A3.Authorization
{
    /// <summary>
    /// A service for managing the replicated activity group data.
    /// </summary>
    public interface IActivityGroupService
    {
        /// <summary>
        /// Returns whether or not this application knows anything about an activity group.
        /// </summary>
        /// <param name="activityGroupName">The activity group name.</param>
        /// <returns>True if this application knows about the activity group, otherwise false.</returns>
        Task<bool> Exists(string activityGroupName);

        /// <summary>
        /// Replaces all activity groups with the latest from the activity group micro-app.
        /// Evicts this application's cache of activity groups.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task SyncActivityGroups();

        /// <summary>
        /// Replaces all activity group grants for user with latest from the activity group micro-app.
        /// Evicts this application's cache of grants for this user.
        /// </summary>
        /// <param name="userId">The user to sync.</param>
        /// <returns>An awaitable task.</returns>
        Task SyncActivityGroupGrants(string userId);

        /// <summary>
        /// Replaces all activity group grants for all users with latest from the activity group micro-app.
        /// Evicts this application's cache of grants for all users.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task SyncAllActivityGroupGrants();
    }
}
