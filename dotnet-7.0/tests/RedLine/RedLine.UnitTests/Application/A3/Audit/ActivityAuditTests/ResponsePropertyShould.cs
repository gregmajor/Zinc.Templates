using System.Threading.Tasks;
using FluentAssertions;
using RedLine.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Audit.ActivityAuditTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ResponsePropertyShould
    {
        [Fact]
        public async Task ReturnResponseToStringValue()
        {
            // Arrange
            const string userId = "test.user";
            const string userName = "Test User";
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(userId, userName, null);
            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);

            // Act
            var response = await activityAudit.Decorate(() => Task.FromResult(new TestActivityResponse())).ConfigureAwait(false);

            // Assert
            ((IActivityAudited)activityAudit).Response.Should().Be(response.ToString());
        }
    }
}
