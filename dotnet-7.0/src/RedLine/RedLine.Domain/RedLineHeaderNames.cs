namespace RedLine.Domain
{
    /// <summary>
    /// Defines the common RedLine header names.
    /// </summary>
    public static class RedLineHeaderNames
    {
        /// <summary>
        /// Gets the correlation id header name.
        /// </summary>
        public static readonly string CorrelationId = "redline-correlation-id";

        /// <summary>
        /// Gets the etag header name.
        /// </summary>
        public static readonly string ETag = "redline-etag";

        /// <summary>
        /// Gets the originating endpoint of a message.
        /// </summary>
        public static readonly string OriginatingEndpoint = "NServiceBus.OriginatingEndpoint";

        /// <summary>
        /// Gets the tenant id header name.
        /// </summary>
        public static readonly string TenantId = "redline-tenant-id";
    }
}
