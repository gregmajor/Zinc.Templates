using System;

namespace Zinc.Templates.Host.Web.Models.Authorization
{
    /// <summary>
    /// The model used to add a grant.
    /// </summary>
    public class AddGrantModel
    {
        /// <summary>
        /// The optional date/time when the grant expires.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// The human readable full name of the grantee, used for audit messages.
        /// </summary>
        public string FullName { get; set; }
    }
}
