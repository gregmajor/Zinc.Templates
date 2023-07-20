using System.IO;
using System.Reflection;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RedLine.Data.Serialization
{
    /// <summary>
    /// An <see cref="IContractResolver"/> that ignores all properties of type <see cref="Stream"/>.
    /// </summary>
    internal class IgnoreStreamContractResolver : PrivateSetterContractResolver
    {
        /// <summary>
        /// Override the <see cref="DefaultContractResolver.CreateProperty" />. to ignore stream to be serialized in audit.</summary>
        /// <param name="member"><see cref="MemberInfo"/>.</param>
        /// <param name="memberSerialization"><see cref="MemberSerialization"/>.</param>
        /// <returns>Property with ignored to be false.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (typeof(Stream).IsAssignableFrom(prop.PropertyType))
            {
                prop.Ignored = true;
            }

            return prop;
        }
    }
}
