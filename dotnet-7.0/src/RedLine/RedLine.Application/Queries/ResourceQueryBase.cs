using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RedLine.Application.Queries
{
    /// <summary>
    /// An abstract base query class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResourceType">The type of resource secured by this request.</typeparam>
    /// <typeparam name="TResponse">The response type returned by the query.</typeparam>
    public abstract class ResourceQueryBase<TResourceType, TResponse> : ResourceQueryBase<TResponse>, IAmAuthorizableForResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceQueryBase{TResourceType, TResponse}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        /// <param name="resourceId">The resource identifier accessed by this query.</param>
        protected ResourceQueryBase(string tenantId, Guid correlationId, string resourceId)
            : base(tenantId, correlationId, resourceId)
        {
        }

        /// <inheritdoc />
        public override string[] ResourceTypes => new[] { TypeNameHelper.GetTypeDisplayName(typeof(TResourceType), false, true) };
    }

    /// <summary>
    /// An abstract base query class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResponse">The response type returned by the query.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The two classes are related and small.")]
    public abstract class ResourceQueryBase<TResponse> : QueryBase<TResponse>, IAmAuthorizableForResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceQueryBase{TResourceType, TResponse}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        /// <param name="resourceId">The resource identifier accessed by this query.</param>
        protected ResourceQueryBase(string tenantId, Guid correlationId, string resourceId)
            : base(tenantId, correlationId)
        {
            ResourceId = resourceId;
        }

        /// <inheritdoc />
        public string ResourceId { get; protected init; }

        /// <inheritdoc />
        public abstract string[] ResourceTypes { get; }
    }
}
