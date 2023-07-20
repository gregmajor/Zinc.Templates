using System;
using System.Threading.Tasks;
using FluentAssertions;
using RedLine.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Audit.ActivityAuditTests
{
    [Collection(nameof(UnitTestCollection))]
    public class DecorateShould
    {
        [Fact]
        public async Task SetTheErrorProperty()
        {
            // Arrange
            const string userId = "test.user";
            const string userName = "Test User";
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(userId, userName, null);

            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);

            var exception = new NotImplementedException();

            // Act/Assert
#pragma warning disable S3626 // Jump statements should not be redundant
            await Assert.ThrowsAsync<NotImplementedException>(() => activityAudit.Decorate(() => throw exception)).ConfigureAwait(false);
#pragma warning restore S3626 // Jump statements should not be redundant
            activityAudit.Exception.Should().Be(exception);
        }

        [Fact]
        public async Task SetTheResponseProperty()
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
            response.Should().NotBeNull();
            activityAudit.Response.Should().Be(response);
        }
    }
}
