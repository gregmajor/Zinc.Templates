using System;
using FluentAssertions;
using RedLine.A3.Audit;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Audit.ActivityAuditTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ConstructorShould
    {
        [Fact]
        public void InitializePropertiesFromIncomingRequest()
        {
            // Arrange
            const string userId = "test.user";
            const string userName = "Test User";
            const string login = "login";
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(userId, userName, login);

            // Act
            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);

            // Assert
            activityAudit.ActivityName.Should().Be(request.GetType().Name);
            activityAudit.ApplicationName.Should().Be(context.ApplicationName());
            activityAudit.CorrelationId.Should().Be(context.CorrelationId());
            activityAudit.Exception.Should().BeNull();
            activityAudit.Request.Should().Be(request);
            activityAudit.Response.Should().BeNull();
            activityAudit.StatusCode.Should().Be(0);
            activityAudit.TenantId.Should().Be(context.TenantId());
            activityAudit.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(250));
            activityAudit.UserId.Should().Be(userId);
            activityAudit.UserName.Should().Be(userName);
            activityAudit.Login.Should().Be(login);
        }
    }
}
