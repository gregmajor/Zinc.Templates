using Xunit;

namespace Zinc.Templates.IntegrationTests.Jobs
{
    [CollectionDefinition(nameof(JobsTestCollection))]
    public class JobsTestCollection : ICollectionFixture<JobsTestFixture>
    {
    }
}
