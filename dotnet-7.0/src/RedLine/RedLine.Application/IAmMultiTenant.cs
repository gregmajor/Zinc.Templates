namespace RedLine.Application
{
    /// <summary>
    ///   The interface that defines a contract for things that are multi-tenant.
    /// </summary>
    public interface IAmMultiTenant
    {
        /// <summary>
        ///   Gets the tenant unique identifier.
        /// </summary>
        string TenantId { get; }
    }
}
