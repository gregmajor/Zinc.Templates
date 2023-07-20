namespace RedLine.Data
{
    /// <summary>
    /// The interface that defines a contract for connection strings.
    /// </summary>
    public interface IConnectionString
    {
        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the connection string.
        /// </summary>
        string Value { get; }
    }
}
