namespace RedLine.Application
{
    /// <summary>
    /// An enum that defines the types of supported activities.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// The activity type is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The activity is a command.
        /// </summary>
        Command,

        /// <summary>
        /// The activity is a query.
        /// </summary>
        Query,

        /// <summary>
        /// The activity is a notification (similar to an event).
        /// </summary>
        Notification,

        /// <summary>
        /// The activity is a job, such as a Quartz job.
        /// </summary>
        Job,
    }
}
