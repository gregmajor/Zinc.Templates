using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using RedLine.A3.Authorization.Domain;
using RedLine.Domain.A3.Authorization.Repositories;
using RedLine.Domain.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.A3.GrantRepositoryTests
{
    public class DeleteShould : FunctionalTestBase
    {
        public DeleteShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ThrowErrorForActiveGrants()
        {
            var scope = new GrantScope("tenant", "type", "qualifier");
            var grant = new Grant("user1", "user one", scope, null, "system");

            var repository = GetRequiredService<IGrantRepository>();

            await repository.Save(grant).ConfigureAwait(false);

            await Assert.ThrowsAsync<DomainException>(() => repository.Delete(grant)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ArchiveExpiredGrants()
        {
            // Arrange
            var userId = "user1";
            var repository = GetRequiredService<IGrantRepository>();

            var expirationDate = DateTimeOffset.UtcNow.AddMilliseconds(50);

            var expired = new Grant(
                userId,
                userId,
                new GrantScope("tenant", "Expired", "100ms"),
                expirationDate,
                "system");

            await repository.Save(expired).ConfigureAwait(false);

            await Task.Delay(70).ConfigureAwait(false);

            // Act
            await repository.Delete(expired).ConfigureAwait(false);

            // Assert
            (await repository.Read(expired.Key).ConfigureAwait(false))
                .Should().BeNull();

            var expected = new GrantData
            {
                TenantId = expired.Scope.TenantId,
                GrantType = expired.Scope.GrantType,
                Qualifier = expired.Scope.Qualifier,
                ExpiresOn = expired.ExpiresOn,
                RevokedBy = (string)null,
                RevokedOn = (DateTimeOffset?)null,
            };

            var actual = (await GetRequiredService<IDbConnection>()
                .QueryAsync<GrantData>(
                    "select tenant_id,grant_type,qualifier,expires_on,revoked_by,revoked_on from authz.grant_history where user_id = @userId",
                    new { userId }).ConfigureAwait(false))
                .SingleOrDefault();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(a => a.ExpiresOn));
            actual.ExpiresOn.Should().BeCloseTo(expirationDate, TimeSpan.FromMilliseconds(50));
        }

        [Fact]
        public async Task ArchiveRevokedGrants()
        {
            // Arrange
            var userId = "user1";
            var repository = GetRequiredService<IGrantRepository>();

            var revoked = new Grant(
                userId,
                userId,
                new GrantScope("tenant", "Revoked", "By Me"),
                null,
                "system");

            await repository.Save(revoked).ConfigureAwait(false);

            // Act
            revoked.Revoke("By Me");
            await repository.Delete(revoked).ConfigureAwait(false);

            // Assert
            (await repository.Read(revoked.Key).ConfigureAwait(false))
                .Should().BeNull();

            var expected = new GrantData
            {
                TenantId = revoked.Scope.TenantId,
                GrantType = revoked.Scope.GrantType,
                Qualifier = revoked.Scope.Qualifier,
                ExpiresOn = revoked.ExpiresOn,
                RevokedBy = revoked.RevokedBy,
                RevokedOn = revoked.RevokedOn,
            };

            var actual = (await GetRequiredService<IDbConnection>()
                .QueryAsync<GrantData>(
                    "select tenant_id,grant_type,qualifier,expires_on,revoked_by,revoked_on from authz.grant_history where user_id = @userId",
                    new { userId }).ConfigureAwait(false))
                .SingleOrDefault();

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.RevokedOn));
            actual.RevokedOn.Value.Should().BeCloseTo(expected.RevokedOn.Value, TimeSpan.FromMilliseconds(50));
        }

        private record GrantData
        {
            public string TenantId { get; init; }

            public string GrantType { get; init; }

            public string Qualifier { get; init; }

            public DateTimeOffset? ExpiresOn { get; init; }

            public string RevokedBy { get; init; }

            public DateTimeOffset? RevokedOn { get; init; }
        }
    }
}
