using RedLine.Domain.Policies;

namespace RedLine.A3.Authorization
{
    /// <summary>
    /// The interface that defines a contract for obtaining a <see cref="IAuthorizationPolicy"/>.
    /// </summary>
    public interface IAuthorizationPolicyProvider : IPolicyProvider<IAuthorizationPolicy>
    {
    }
}
