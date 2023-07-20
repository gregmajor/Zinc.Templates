using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace RedLine.Application.Commands
{
    /// <summary>
    /// An abstract base command class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResourceType">The type of resource secured by this command.</typeparam>
    /// <remarks>Inherit from this base class when the command does NOT return a response.</remarks>
    public abstract class ResourceCommandBase<TResourceType> : ResourceCommandBase<TResourceType, Unit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCommandBase{TResourceType}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        /// <param name="resourceId">The resource identifier accessed by this command.</param>
        protected ResourceCommandBase(string tenantId, Guid correlationId, string resourceId)
            : base(tenantId, correlationId, resourceId)
        {
        }
    }

    /// <summary>
    /// An abstract base command class that provides implementations of various required interfaces.
    /// </summary>
    /// <typeparam name="TResourceType">The type of resource secured by this command.</typeparam>
    /// <typeparam name="TResponse">The response type returned by the command.</typeparam>
    /// <remarks>Inherit from this base class when the command returns a response.</remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "The two classes are related and small.")]
    public abstract class ResourceCommandBase<TResourceType, TResponse> : CommandBase<TResponse>, IAmAuthorizableForResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCommandBase{TResourceType, TResponse}"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">A unique correlation identifier provided by the client.</param>
        /// <param name="resourceId">The resource identifier accessed by this command.</param>
        protected ResourceCommandBase(string tenantId, Guid correlationId, string resourceId)
            : base(tenantId, correlationId)
        {
            ResourceId = resourceId;
        }

        /// <inheritdoc />
        public string ResourceId { get; protected init; }

        /// <inheritdoc />
        public virtual string[] ResourceTypes => new[] { TypeNameHelper.GetTypeDisplayName(typeof(TResourceType), false, true) };
    }
}
