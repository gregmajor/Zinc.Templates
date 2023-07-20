using FluentAssertions;
using Newtonsoft.Json;
using RedLine.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Audit.ActivityAuditTests
{
    [Collection(nameof(UnitTestCollection))]
    public class RequestPropertyShould
    {
        [Fact]
        public void ReturnTheSerializedRequest()
        {
            // Arrange
            const string userId = "test.user";
            const string userName = "Test User";
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(userId, userName, null);

            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);

            var expected = JsonConvert.SerializeObject(request);

            // Act
            var actual = ((IActivityAudited)activityAudit).Request;

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
