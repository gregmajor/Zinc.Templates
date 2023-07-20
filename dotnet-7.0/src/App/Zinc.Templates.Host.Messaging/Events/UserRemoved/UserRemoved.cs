using System;
using RedLine.Domain.Events;

namespace Krypton.Authentication.Domain.Events
{
    /// <summary>
    /// Domain event from Krypton.Authentication.
    /// </summary>
    public class UserRemoved : IDomainEvent
    {
        /// <summary>
        /// The user ID who is removed.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The user name being removed.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user ID performing the action.
        /// </summary>
        public string RemovedBy { get; set; }

        /// <summary>
        /// The UTC Date and time which the user has been removed.
        /// </summary>
        public DateTime RemovedOn { get; set; }
    }
}
