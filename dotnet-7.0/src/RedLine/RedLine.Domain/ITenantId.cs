namespace RedLine.Domain
{
    /// <summary>
    /// The interface that defines a contract for the request tenant identifier.
    /// </summary>
    public interface ITenantId
    {
        /// <summary>
        /// Gets the tenant identifier for the request.
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// An injectable tenant identifier, which is registered in the container by the hosting process.
    /// </summary>
    public sealed class TenantId : ITenantId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantId"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        public TenantId(string tenantId)
        {
            Value = tenantId ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantId"/> class.
        /// </summary>
        public TenantId()
        {
            Value = string.Empty;
        }

        /// <inheritdoc />
        public string Value { get; set; }

        /// <inheritdoc />
        public override string ToString() => Value;
    }
}
