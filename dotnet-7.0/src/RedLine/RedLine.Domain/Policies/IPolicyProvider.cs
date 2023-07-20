using System.Threading.Tasks;

namespace RedLine.Domain.Policies
{
    /// <summary>
    /// Defines a contract for providing policies. Originally created for loading OPA policies where holding onto the provider in memory is beneficial.
    /// </summary>
    /// <typeparam name="TPolicy">The type of policy to provide.</typeparam>
    public interface IPolicyProvider<TPolicy>
        where TPolicy : IPolicy
    {
        /// <summary>
        /// Retrieves the policy according to its requirements.
        /// </summary>
        /// <returns>A policy that can be evaluated.</returns>
        Task<TPolicy> GetPolicy();
    }
}
