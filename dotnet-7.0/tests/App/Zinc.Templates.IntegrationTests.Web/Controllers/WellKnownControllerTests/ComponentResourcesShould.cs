using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.WellKnownControllerTests
{
    public class ComponentResourcesShould : WebTestBase
    {
        private const string Endpoint = "/.well-known/web-component-resources";

        public ComponentResourcesShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task Return500()
        {
            await AuthorizedScenario(_ =>
            {
                _.Get.Url("/.well-known/invalid-component-resources");
                _.StatusCodeShouldBe(500);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnNotAuthorized()
        {
            await AnonymousScenario(_ =>
            {
                _.Get.Url(Endpoint);
                _.StatusCodeShouldBe(401);
            }).ConfigureAwait(false);
        }
    }
}
