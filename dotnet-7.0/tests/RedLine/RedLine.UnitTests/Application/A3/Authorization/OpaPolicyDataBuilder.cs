using System;
using System.Collections.Generic;
using System.Linq;
using RedLine.Application.A3.Authorization.PolicyData;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    public class OpaPolicyDataBuilder
    {
        private readonly IDictionary<string, Activities.Data> activities = new Dictionary<string, Activities.Data>();
        private readonly IDictionary<string, ActivityGroups.Data> activityGroups = new Dictionary<string, ActivityGroups.Data>();
        private readonly IDictionary<string, Grants.Data> grants = new Dictionary<string, Grants.Data>();

        public OpaPolicyDataBuilder WithActivity(string activityName, params string[] resourceTypes)
        {
            activities.Add(activityName, new(resourceTypes));
            return this;
        }

        public OpaPolicyDataBuilder WithActivityGroup(string activityGroupName, params (string TenantId, string ActivityName)[] activities)
        {
            activityGroups.Add(activityGroupName, new(activities.Select(a => new ActivityGroups.ActivityGroupActivity(a.TenantId, a.ActivityName)).ToList()));
            return this;
        }

        public OpaPolicyDataBuilder WithGrant(string grant, DateTime? expiresOn)
        {
            grants.Add(grant, new(expiresOn));
            return this;
        }

        public object BuildData() => new { activities, activityGroups, grants };
    }
}
