using System.Collections.Generic;
using System.Threading.Tasks;
using RedLine.A3.Authorization.Domain;
using RedLine.Domain.Repositories;

namespace RedLine.Domain.A3.Authorization.Repositories
{
    /// <summary>
    /// The interface that defines a contract for persistence of <see cref="Grant"/>s.
    /// </summary>
    public interface IGrantRepository : IRepository<Grant>
    {
        /// <summary>
        /// Reads all grants matching the specified scope and optional user id.
        /// </summary>
        /// <param name="scope">The grant scope to match, which can contain '%' wildcards.</param>
        /// <param name="userId">The optional user id to match.</param>
        /// <returns>The matching grants, if any.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "By design.")]
        Task<IEnumerable<Grant>> Matching(GrantScope scope, string userId = null);

        /// <summary>
        /// Reads all grants associated with a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The grants for the user, if any.</returns>
        Task<IEnumerable<Grant>> ReadAll(string userId);
    }
}
