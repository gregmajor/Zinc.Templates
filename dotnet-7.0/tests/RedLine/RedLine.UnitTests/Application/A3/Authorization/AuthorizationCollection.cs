using Xunit;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    [CollectionDefinition(nameof(AuthorizationCollection))]
    public class AuthorizationCollection : ICollectionFixture<AuthorizationFixture>
    {
    }
}
