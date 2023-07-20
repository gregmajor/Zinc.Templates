using System;

namespace RedLine.Application.Queries.UXGetGrantableActivities
{
    /// <summary>
    /// An activity from an application which can be granted.
    /// </summary>
    public record UXGrantableActivity
    {
        /// <summary>
        /// The user friendly description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The user friendly display name.
        /// </summary>
        public string DisplayName { get; init; }

        /// <summary>
        /// The Name of the Activity or ActivityGroup.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the expiration date of the grant, or null if it does not expire.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; init; }

        /// <summary>
        /// Gets the user who bestowed the grant.
        /// </summary>
        public string GrantedBy { get; init; }

        /// <summary>
        /// Gets the date/time when the grant was bestowed.
        /// </summary>
        public DateTimeOffset? GrantedOn { get; init; }
    }
}
