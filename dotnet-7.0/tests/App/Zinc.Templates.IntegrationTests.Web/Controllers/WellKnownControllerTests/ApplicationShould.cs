using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.WellKnownControllerTests
{
    public class ApplicationShould : WebTestBase
    {
        private const string Endpoint = "/.well-known/application";

        public ApplicationShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnApplicationInformation()
        {
            var response = await AuthenticatedScenario(_ =>
            {
                _.Get.Url(Endpoint);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var result = response.ReadAsJson<ApplicationResult>();
            result.Should().NotBeNull();
            result.Name.Should().Be(ApplicationContext.ApplicationName);
            result.DisplayName.Should().Be(ApplicationContext.ApplicationDisplayName);
            result.Activities.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ReturnNotAuthenticated()
        {
            await AnonymousScenario(_ =>
            {
                _.Get.Url(Endpoint);
                _.StatusCodeShouldBe(401);
            }).ConfigureAwait(false);
        }

        internal record ApplicationResult
        {
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public ActivityResult[] Activities { get; set; }
        }

        internal record ActivityResult
        {
            public string Name { get; set; }

            public string DisplayName { get; set; }

            public string Description { get; set; }
        }
    }
}
