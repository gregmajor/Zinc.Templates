using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RedLine.A3;
using RedLine.Application;
using RedLine.Domain;

namespace RedLine.UnitTests.Application.A3.Audit
{
    internal static class TestActivityContext
    {
        public static readonly Mock<IDbConnection> DbMock = new Mock<IDbConnection>();

        public static readonly TenantId TestTenant = new TenantId("test");

        public static IAccessToken AccessToken(string userId, string fullName, string login)
        {
            var mock = new Mock<IAccessToken>();
            mock.Setup(x => x.UserId).Returns(userId);
            mock.Setup(x => x.FullName).Returns(fullName);
            mock.Setup(x => x.Login).Returns(login);
            return mock.Object;
        }

        public static ActivityContext NewContext(string userId, string fullName, string login)
        {
            var context = new ActivityContext(
                TestTenant,
                new CorrelationId(),
                new ETag(),
                DbMock.Object,
                AccessToken(userId, fullName, login),
                new ClientAddress(),
                new ServiceCollection().BuildServiceProvider());

            context.Set(nameof(ApplicationContext.ApplicationName), "RedLine");

            return context;
        }
    }
}
