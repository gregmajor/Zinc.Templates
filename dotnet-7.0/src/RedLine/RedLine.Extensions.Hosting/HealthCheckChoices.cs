namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// Provides options for health check extensions.
    /// </summary>
    public class HealthCheckChoices
    {
        /// <summary>
        /// If true, disable the memory check.
        /// </summary>
        public bool DisableMemoryCheck { get; set; } = false;

        /// <summary>
        /// If true, disable the authz check.
        /// </summary>
        public bool DisableAuthZCheck { get; set; } = false;

        /// <summary>
        /// If true, disable the authn cert check.
        /// </summary>
        public bool DisableAuthNCertCheck { get; set; } = false;

        /// <summary>
        /// If true, disable the postgres check.
        /// </summary>
        public bool DisablePostgresCheck { get; set; } = false;

        /// <summary>
        /// If true, disable the rabbitmq check.
        /// </summary>
        public bool DisableRabbitCheck { get; set; } = false;
    }
}
