using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Host.Web.Models.Authorization;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GrantControllerTests
{
    public class RevokeGrantShould : WebTestBase
    {
        private static readonly string ActivityGrantEndpoint = $"/api/v1/{TenantId}/activities/FooActivity/grants/{Guid.NewGuid()}";

        public RevokeGrantShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task RevokeTheGrant()
        {
            var model = new AddGrantModel { ExpiresOn = null, FullName = "Full Name" };

            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            await AuthorizedScenario(_ =>
            {
                _.Delete.Url(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnNotFoundIfGrantDoesNotExist()
        {
            await AuthorizedScenario(_ =>
            {
                _.Delete.Url(ActivityGrantEndpoint);
                _.StatusCodeShouldBe(404);
            }).ConfigureAwait(false);
        }
    }
}
