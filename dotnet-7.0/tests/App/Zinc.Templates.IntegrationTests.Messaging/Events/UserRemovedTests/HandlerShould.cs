using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Krypton.Authentication.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedLine.A3.Authorization;
using RedLine.A3.Authorization.Domain;
using RedLine.Application;
using RedLine.Domain;
using RedLine.Domain.A3.Authorization.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Host.Messaging.Events.UserRemoved;

namespace Zinc.Templates.IntegrationTests.Messaging.Events.UserRemovedTests
{
    public class HandlerShould : MessagingTestBase
    {
        public HandlerShould(MessagingTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task RevokeAllGrantsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var repo = GetRequiredService<IGrantRepository>();
            var cache = GetRequiredService<IDistributedCache>();
            var activities = GetRequiredService<IEnumerable<IActivity>>().ToList();
            var grants = activities.Select(a => new Grant(userId, "fullName", TenantId, GrantType.Activity, a.ActivityName, null, "me"));

            foreach (var grant in grants)
            {
                await repo.Save(grant).ConfigureAwait(false);
            }

            await cache.SetStringAsync(AuthorizationCacheKey.ForGrants(userId), "{}").ConfigureAwait(false);

            var userRemovedEvent = new UserRemoved
            {
                UserId = userId,
                UserName = "fullName",
                RemovedBy = "me",
                RemovedOn = DateTime.UtcNow,
            };

            var handler = new UserRemovedHandler(GetRequiredService<IMediator>(), GetRequiredService<ITenantId>(), GetRequiredService<ICorrelationId>(), GetRequiredService<ILogger<UserRemovedHandler>>());

            // Act
            await handler.Handle(userRemovedEvent, null).ConfigureAwait(false);

            // Assert
            grants = await repo.ReadAll(userId).ConfigureAwait(false);
            grants.Should().BeEmpty();
            var cached = await cache.GetStringAsync(AuthorizationCacheKey.ForGrants(userId)).ConfigureAwait(false);
            cached.Should().BeNull();
        }
    }
}
