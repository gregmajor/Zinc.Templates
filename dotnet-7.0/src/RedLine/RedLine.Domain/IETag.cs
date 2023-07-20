namespace RedLine.Domain
{
    /// <summary>
    /// The concurrency identifier, aka entity tag or ETag, for the request.
    /// </summary>
    public interface IETag
    {
        /// <summary>
        /// Gets the request ETag value.
        /// </summary>
        string IncomingValue { get; set; }

        /// <summary>
        /// Gets the response ETag value.
        /// </summary>
        string OutgoingValue { get; set; }
    }

    /// <summary>
    /// Provides an implementation of the <see cref="IETag"/> interface.
    /// </summary>
    public sealed class ETag : IETag
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ETag()
        {
            IncomingValue = string.Empty;
            OutgoingValue = string.Empty;
        }

        /// <inheritdoc/>
        public string IncomingValue { get; set; }

        /// <inheritdoc />
        public string OutgoingValue { get; set; }
    }
}
