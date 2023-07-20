using System;
using System.Threading.Tasks;
using FluentAssertions;
using Krypton.Audit;
using RedLine.A3.Audit;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Audit.AuditEventTests
{
    [Collection(nameof(UnitTestCollection))]
    public class ConstructorShould
    {
        [Fact]
        public async Task InitializePropertiesFromRequestOnError()
        {
            // Arrange
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(null, null, null);
            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);
            var customExceptionMessage = "custom test exception";

            Func<Task<TestActivityResponse>> someAction = async () =>
            {
                await Task.CompletedTask.ConfigureAwait(false);
                throw new NotImplementedException(customExceptionMessage);
            };

            // Act
            Func<Task<TestActivityResponse>> decorate = () => activityAudit.Decorate(someAction);
            await decorate.Should().ThrowAsync<NotImplementedException>().WithMessage(customExceptionMessage).ConfigureAwait(false);
            var auditEvent = new ActivityAudited(activityAudit);

            // Assert
            auditEvent.ActivityName.Should().Be(activityAudit.Request.GetType().Name);
            auditEvent.ApplicationName.Should().Be(activityAudit.ApplicationName);
            auditEvent.CorrelationId.Should().Be(activityAudit.CorrelationId);
            auditEvent.Exception.Should().Contain(customExceptionMessage);
            auditEvent.Request.Should().Be(((IActivityAudited)activityAudit).Request);
            auditEvent.Response.Should().BeNull();
            auditEvent.StatusCode.Should().Be(activityAudit.StatusCode);
            auditEvent.TenantId.Should().Be(activityAudit.TenantId);
            auditEvent.Timestamp.Should().Be(activityAudit.Timestamp);
            auditEvent.UserId.Should().Be(activityAudit.UserId);
            auditEvent.UserName.Should().Be(activityAudit.UserName);
        }

        [Fact]
        public async Task InitializePropertiesFromRequestOnSuccess()
        {
            // Arrange
            var request = new TestActivityRequest();
            var context = TestActivityContext.NewContext(null, null, null);
            var activityAudit = new ActivityAudit<TestActivityRequest, TestActivityResponse>(context, request);
            await activityAudit.Decorate(() => Task.FromResult(new TestActivityResponse())).ConfigureAwait(false);

            // Act
            var auditEvent = new ActivityAudited(activityAudit);

            // Assert
            auditEvent.ActivityName.Should().Be(activityAudit.Request.GetType().Name);
            auditEvent.ApplicationName.Should().Be(activityAudit.ApplicationName);
            auditEvent.CorrelationId.Should().Be(activityAudit.CorrelationId);
            auditEvent.Exception.Should().BeNull();
            auditEvent.Request.Should().Be(((IActivityAudited)activityAudit).Request);
            auditEvent.Response.Should().Be(activityAudit.Response.ToString());
            auditEvent.StatusCode.Should().Be(activityAudit.StatusCode);
            auditEvent.TenantId.Should().Be(activityAudit.TenantId);
            auditEvent.Timestamp.Should().Be(activityAudit.Timestamp);
            auditEvent.UserId.Should().Be(activityAudit.UserId);
            auditEvent.UserName.Should().Be(activityAudit.UserName);
        }
    }
}
