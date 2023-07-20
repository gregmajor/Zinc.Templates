using System;
using Dapper;
using RedLine.Data.Repositories;

namespace Zinc.Templates.Data.DataQueries.UXGreetingsScreen
{
    /// <summary>
    /// Runs a UX Query.
    /// </summary>
    public class UXGreetingDataQuery : DbDataQuery<(string Message, string ETag, DateTimeOffset Timestamp)>
    {
        /// <summary>
        /// Initializes a ux query.
        /// </summary>
        /// <param name="key">The greeting id.</param>
        /// <param name="tenantId">The tenant id.</param>
        public UXGreetingDataQuery(Guid key, string tenantId)
        {
            var sql = "SELECT message, etag, timestamp FROM app.greetings WHERE greeting_id = @key AND tenantid = @tenantId";
            Resolve = connection => connection.QuerySingleOrDefaultAsync<(string, string, DateTimeOffset)>(sql, new { key, tenantId });
        }
    }
}
