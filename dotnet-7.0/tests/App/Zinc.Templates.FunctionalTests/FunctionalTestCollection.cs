using Xunit;

namespace Zinc.Templates.FunctionalTests
{
    [CollectionDefinition(nameof(FunctionalTestCollection))]
    public class FunctionalTestCollection : ICollectionFixture<FunctionalTestFixture>
    {
    }
}
