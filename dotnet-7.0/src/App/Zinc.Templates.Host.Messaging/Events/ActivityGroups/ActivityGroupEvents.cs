using System;
using System.Diagnostics.CodeAnalysis;
using RedLine.Domain.Events;

namespace Krypton.ActivityGroups.Domain.Events
{
    /// <summary>
    /// The domain event raised when an Activity has been added to an ActivityGroup.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "The file contains more than one event.")]
    public class ActivityAddedToGroup : IDomainEvent
    {
        /// <summary>
        /// Gets the unique key for this activity group.
        /// </summary>
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// Gets the application which owns the activity.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the activity which was added to the group.
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// The user who added the activity to the group.
        /// </summary>
        public string AddedBy { get; set; }

        /// <summary>
        /// The date/time the activity was added to the group.
        /// </summary>
        public DateTimeOffset AddedOn { get; set; }
    }

    /// <summary>
    /// The event that is raised when an activity group is deleted.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The events are related and small.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "The file contains more than one event.")]
    public class ActivityGroupDeleted : IDomainEvent
    {
        /// <summary>
        /// The name of the activity group.
        /// </summary>
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// The name of the user who deleted the activity group.
        /// </summary>
        public string DeletedBy { get; set; }

        /// <summary>
        /// The date/time when the activity group was deleted.
        /// </summary>
        public DateTimeOffset DeletedOn { get; set; }
    }

    /// <summary>
    /// The domain event raised when an Activity has been removed from an ActivityGroup.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The events are related and small.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "The file contains more than one event.")]
    public class ActivityRemovedFromGroup : IDomainEvent
    {
        /// <summary>
        /// Gets the unique key for this activity group.
        /// </summary>
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// Gets the application which owns the activity.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the activity which was removed from the group.
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// The user who removed the group.
        /// </summary>
        public string RemovedBy { get; set; }

        /// <summary>
        /// The date/time when the activity was removed.
        /// </summary>
        public DateTimeOffset RemovedOn { get; set; }
    }

    /// <summary>
    /// The event that is raised when an ActivityGroup has been granted to a user.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The events are related and small.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "The file contains more than one event.")]
    public class ActivityGroupGranted : IDomainEvent
    {
        /// <summary>
        /// The name of the activity group that was granted.
        /// </summary>
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// Gets the date/time when the grant expires, or null if it does not expire.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// The full name of the grantee.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the user who bestowed the grant.
        /// </summary>
        public string GrantedBy { get; set; }

        /// <summary>
        /// Gets the date/time when the grant was bestowed.
        /// </summary>
        public DateTimeOffset GrantedOn { get; set; }

        /// <summary>
        /// Gets the type of grant that was bestowed.
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets the qualifier for the grant that was bestowed.
        /// </summary>
        public string Qualifier { get; set; }

        /// <summary>
        /// Gets the tenant for which the grant was bestowed.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets the user id of the grantee.
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// The event that is raised when an ActivityGroup has been revoked from a user.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The events are related and small.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:FileNameShouldMatchFirstTypeName", Justification = "The file contains more than one event.")]
    public class ActivityGroupRevoked : IDomainEvent
    {
        /// <summary>
        /// The name of the activity group that was revoked.
        /// </summary>
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// Gets the date/time when the grant expires, or null if it does not expire.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// The full name of the grantee.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the user who bestowed the grant.
        /// </summary>
        public string GrantedBy { get; set; }

        /// <summary>
        /// Gets the date/time when the grant was bestowed.
        /// </summary>
        public DateTimeOffset GrantedOn { get; set; }

        /// <summary>
        /// Gets the type of grant that was bestowed.
        /// </summary>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets the qualifier for the grant that was bestowed.
        /// </summary>
        public string Qualifier { get; set; }

        /// <summary>
        /// Gets the tenant for which the grant was bestowed.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets the user id of the grantee.
        /// </summary>
        public string UserId { get; set; }
    }
}
