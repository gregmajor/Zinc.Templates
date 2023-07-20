using Newtonsoft.Json;

namespace RedLine.Application
{
    /// <summary>
    /// The interface that defines a contract for commands and queries that must be authorized against a specific resource.
    /// </summary>
    public interface IAmAuthorizableForResource : IAmAuthorizable
    {
        /// <summary>
        /// Gets the identifier for the resource to be authorized against.
        /// </summary>
        string ResourceId { get; }

        /// <summary>
        /// Gets the types of resource to be authorized against.
        /// </summary>
        /// <returns>The types of resources.</returns>
        [JsonIgnore]
        string[] ResourceTypes { get; }
   }
}
