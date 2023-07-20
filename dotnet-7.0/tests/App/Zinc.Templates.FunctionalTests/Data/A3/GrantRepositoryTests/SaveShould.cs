using System;
using System.Threading.Tasks;
using RedLine.A3.Authorization.Domain;
using RedLine.Data.Exceptions;
using RedLine.Domain.A3.Authorization.Repositories;
using RedLine.Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.A3.GrantRepositoryTests
{
    public class SaveShould : FunctionalTestBase
    {
        public SaveShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ThrowWhenGrantIsRevoked()
        {
            // Arrange
            var repository = GetRequiredService<IGrantRepository>();

            var revoked = new Grant(
                "user1",
                "user one",
                new GrantScope("tenant", "type2", "qualifier2"),
                null,
                "system");

            revoked.Revoke("by me");

            // Act/Assert
            await Assert.ThrowsAsync<DomainException>(() => repository.Save(revoked)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ThrowWhenGrantIsExpired()
        {
            // Arrange
            var repository = GetRequiredService<IGrantRepository>();

            var expired = new Grant(
                "user1",
                "user one",
                new GrantScope("tenant", "type", "qualifier1"),
                DateTimeOffset.UtcNow.AddMilliseconds(10),
                "system");

            await Task.Delay(20).ConfigureAwait(false);

            // Act/Assert
            await Assert.ThrowsAsync<DomainException>(() => repository.Save(expired)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ThrowResourceAlreadyExistsExceptionOnKeyViolation()
        {
            // Arrange
            var repository = GetRequiredService<IGrantRepository>();
            var userId = "user1";
            var scope = new GrantScope("tenant", "type", "qualifier");

            var grant = new Grant(userId, userId, scope, null, "me");
            await repository.Save(grant).ConfigureAwait(false);

            // Act/Assert
            await Assert.ThrowsAsync<ResourceAlreadyExistsException>(() => repository.Save(grant)).ConfigureAwait(false);
        }
    }
}
