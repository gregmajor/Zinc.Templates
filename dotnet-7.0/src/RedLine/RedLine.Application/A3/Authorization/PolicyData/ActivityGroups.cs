using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace RedLine.Application.A3.Authorization.PolicyData
{
    /// <summary>
    /// This represents the activity group data set in the authorization rego.
    /// <code>
    /// activityGroups: {
    ///     "NameOf::AnActivityGroup": {
    ///         "activities": ["ActivityName"]
    ///     }
    /// }
    /// </code>
    /// </summary>
    internal class ActivityGroups
    {
        private readonly IDbConnection connection;

        public ActivityGroups(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IDictionary<string, object>> Value()
        {
            var activityGroups = await connection.QueryAsync<(string Name, string TenantId, string ActivityName)>(
                    "SELECT name, tenant_id, activity_name FROM authz.activity_group").ConfigureAwait(false);

            return activityGroups
                .GroupBy(g => g.Name, g => new ActivityGroupActivity(g.TenantId, g.ActivityName))
                .ToDictionary(g => g.Key, g => (object)new Data(g.ToList()));
        }

        internal record Data
        {
            public Data(IList<ActivityGroupActivity> activities)
            {
                Activities = activities;
            }

            public IList<ActivityGroupActivity> Activities { get; }
        }

        internal record ActivityGroupActivity
        {
            public ActivityGroupActivity(string tenantId, string activityName)
            {
                TenantId = tenantId;
                ActivityName = activityName;
            }

            public string TenantId { get; }

            public string ActivityName { get; }
        }
    }
}
