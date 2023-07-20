namespace RedLine.Domain
{
    /// <summary>
    /// The interface that defines a contract for the client IP address or host name.
    /// </summary>
    public interface IClientAddress
    {
        /// <summary>
        /// Gets or sets the value of the remote client IP address or host name.
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// Provides an implementation of <see cref="IClientAddress"/>.
    /// </summary>
    public class ClientAddress : IClientAddress
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="value">The IP address or host name value.</param>
        public ClientAddress(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ClientAddress()
        { }

        /// <inheritdoc/>
        public string Value { get; set; }
    }
}
