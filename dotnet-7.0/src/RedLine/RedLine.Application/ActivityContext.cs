using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RedLine.A3;
using RedLine.Domain;

namespace RedLine.Application
{
    /// <summary>
    /// Provides an implementation of <see cref="IActivityContext"/>.
    /// </summary>
    public class ActivityContext : IActivityContext
    {
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();

        private readonly string[] injectedValueNames = new[]
        {
            nameof(TenantId),
            nameof(CorrelationId),
            nameof(ETag),
            nameof(IDbConnection),
            nameof(AccessToken),
            nameof(ClientAddress),
            nameof(IServiceProvider),
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The <see cref="ITenantId"/> for the current request.</param>
        /// <param name="correlationId">The <see cref="ICorrelationId"/> for the current request.</param>
        /// <param name="etag">The <see cref="ETag"/> for the current request.</param>
        /// <param name="connection">The <see cref="IDbConnection"/> to the database.</param>
        /// <param name="accessToken">The <see cref="IAccessToken"/> for the current user.</param>
        /// <param name="clientAddress">The <see cref="IClientAddress"/> for the current request.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for the current request.</param>
        public ActivityContext(
            ITenantId tenantId,
            ICorrelationId correlationId,
            IETag etag,
            IDbConnection connection,
            IAccessToken accessToken,
            IClientAddress clientAddress,
            IServiceProvider serviceProvider)
        {
            data.Add(nameof(TenantId), tenantId);
            data.Add(nameof(CorrelationId), correlationId);
            data.Add(nameof(ETag), etag);
            data.Add(nameof(IDbConnection), connection);
            data.Add(nameof(AccessToken), accessToken);
            data.Add(nameof(ClientAddress), clientAddress);
            data.Add(nameof(IServiceProvider), serviceProvider);
        }

        /// <inheritdoc />
        public T Get<T>(string key, T defaultValue)
        {
            return data.ContainsKey(key) ? (T)data[key] : defaultValue;
        }

        /// <inheritdoc />
        public void Set<T>(string key, T value)
        {
            if (injectedValueNames.Any(x => x.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"'{key}' is an injected value and cannot be replaced.");
            }

            data[key] = value;
        }
    }
}
