namespace RedLine.Extensions.Hosting
{
    /// <summary>
    /// An enum that defines the type of endpoint to configure.
    /// </summary>
    public enum EndpointType
    {
        /// <summary>
        /// The endpoint should be configured as send-only.
        /// </summary>
        SendOnly,

        /// <summary>
        /// The endpoint should be configured as full-duplex (send and receive).
        /// </summary>
        FullDuplex,
    }
}
