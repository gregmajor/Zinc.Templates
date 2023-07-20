using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Host.Web.Models.Authorization;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GrantControllerTests
{
    public class AddGrantShould : WebTestBase
    {
        private static readonly string ActivityGrantEndpoint = $"/api/v1/{TenantId}/activities/FooActivity/grants/{Guid.NewGuid()}";

        public AddGrantShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task AddTheGrant()
        {
            var model = new AddGrantModel { ExpiresOn = null, FullName = "Full Name" };

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task AddGrantIfExpiredGrantExists()
        {
            await AuthorizedScenario(_ =>
            {
                _.Post.Json(new AddGrantModel { ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(5), FullName = "Full Name" }).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromSeconds(6)).ConfigureAwait(false);

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(new AddGrantModel { ExpiresOn = null, FullName = "Full Name" }).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnBadRequestIfGrantExists()
        {
            var model = new AddGrantModel { ExpiresOn = null, FullName = "Full Name" };

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(400);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnBadRequestIfNonExpiredGrantExists()
        {
            var model = new AddGrantModel { ExpiresOn = DateTimeOffset.UtcNow.AddDays(1), FullName = "Full Name" };

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(400);
            }).ConfigureAwait(false);
        }
    }
}
