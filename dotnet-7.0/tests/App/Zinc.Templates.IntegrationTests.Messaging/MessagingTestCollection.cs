using Xunit;

namespace Zinc.Templates.IntegrationTests.Messaging
{
    [CollectionDefinition(nameof(MessagingTestCollection))]
    public class MessagingTestCollection : ICollectionFixture<MessagingTestFixture>
    {
    }
}
