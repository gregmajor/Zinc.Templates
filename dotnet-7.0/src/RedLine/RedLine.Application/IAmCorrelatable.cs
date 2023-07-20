using System;

namespace RedLine.Application
{
    /// <summary>
    ///   A marker interface that indicates a request can be correlated across services using a unique identifier.
    /// </summary>
    /// <remarks>The client of a given service is responsible for creating the correlation identifier.</remarks>
    public interface IAmCorrelatable
    {
        /// <summary>
        ///   Gets the correlation identifier for the request.
        /// </summary>
        Guid CorrelationId { get; }
    }
}
