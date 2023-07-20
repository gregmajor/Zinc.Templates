using Xunit;

namespace Zinc.Templates.IntegrationTests.Web
{
    [CollectionDefinition(nameof(WebTestCollection))]
    public class WebTestCollection : ICollectionFixture<WebTestFixture>
    {
    }
}
