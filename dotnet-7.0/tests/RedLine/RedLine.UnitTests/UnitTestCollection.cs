using Xunit;

namespace RedLine.UnitTests
{
    [CollectionDefinition(nameof(UnitTestCollection))]
    public class UnitTestCollection : ICollectionFixture<UnitTestFixture>
    {
    }
}
