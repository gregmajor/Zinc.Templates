using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace RedLine.Application.A3.Authorization.PolicyData
{
    /// <summary>
    /// This represents the grant data set in the authorization rego.
    /// <code>
    /// grants: {
    ///     "Grant:Scope": {
    ///         "expiresOn": 3456787654
    ///     }
    /// }
    /// </code>
    /// </summary>
    internal class Grants
    {
        private readonly IDbConnection connection;

        public Grants(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IDictionary<string, object>> Value(string userId)
        {
            var grants = await connection.QueryAsync<(string TenantId, string GrantType, string Qualifier, DateTimeOffset? ExpiresOn)>(
                "SELECT tenant_id, grant_type, qualifier, expires_on::timestamptz FROM authz.\"grant\" WHERE user_id = @userId",
                new { userId }).ConfigureAwait(false);

            return grants.ToDictionary(g => string.Join(":", g.TenantId, g.GrantType, g.Qualifier), g => (object)new Data(g.ExpiresOn));
        }

        internal record Data
        {
            public Data(DateTimeOffset? expiresOn)
            {
                ExpiresOn = expiresOn?.ToUniversalTime().Ticks;
            }

            public long? ExpiresOn { get; }
        }
    }
}
