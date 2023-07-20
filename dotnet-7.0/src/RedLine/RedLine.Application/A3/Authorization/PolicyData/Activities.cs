using System.Collections.Generic;
using System.Linq;

namespace RedLine.Application.A3.Authorization.PolicyData
{
    /// <summary>
    /// This represents the activities data set in the authorization rego.
    /// <code>
    /// activities: {
    ///     "ActivityName": {
    ///         "resourceTypes": ["ResourceType"]
    ///     }
    /// }
    /// </code>
    /// </summary>
    internal class Activities
    {
        public Activities(IEnumerable<IActivity> activities)
        {
            Value = activities.ToDictionary(
                a => a.ActivityName,
                a => new Data((a as IAmAuthorizableForResource)?.ResourceTypes));
        }

        public IDictionary<string, Data> Value { get; }

        internal record Data
        {
            public Data(IList<string> resourceTypes)
            {
                if (resourceTypes?.Count != 0)
                {
                    ResourceTypes = resourceTypes;
                }
            }

            public IList<string> ResourceTypes { get; }
        }
    }
}
