using Newtonsoft.Json;

namespace RedLine.Application
{
    /// <summary>
    /// The interface that defines a contract for activities, such as commands, queries, notifications (events) and jobs.
    /// </summary>
    public interface IActivity
        : IAmMultiTenant,
          IAmCorrelatable,
          IAmAuditable,
          IAmAuthorizable,
          IAmTransactional
    {
        /// <summary>
        /// Gets the <see cref="Application.ActivityType"/> of the executing activity.
        /// </summary>
        ActivityType ActivityType { get; }

        /// <summary>
        /// Gets the proper name for the activity.
        /// </summary>
        [JsonIgnore]
        string ActivityName { get; }

        /// <summary>
        /// Gets a user friendly display name for the activity.
        /// </summary>
        [JsonIgnore]
        string ActivityDisplayName { get; }

        /// <summary>
        /// Gets a brief description of the activity.
        /// </summary>
        [JsonIgnore]
        string ActivityDescription { get; }
    }
}
