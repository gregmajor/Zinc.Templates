using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using RedLine.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Application.Queries.UXGreetingsScreen.UXGreeting;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.UXGreetingScreenControllerTests
{
    public class UXGreetingShould : WebTestBase
    {
        private static readonly string Endpoint = $"/ux/v1/{TenantId}/greetings";

        public UXGreetingShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnModel()
        {
            // Arrange
            Greeting expected = GreetingMother.HiThere();
            var repository = GetRequiredService<IRepository<Greeting>>();
            await repository.Save(expected).ConfigureAwait(false);

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(string.Join('/', Endpoint, expected.Key));

                _.StatusCodeShouldBe(200);
                _.Header(HeaderNames.ETag).ShouldNotBeWritten();
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<UXGreetingResult>();
            result.Should().NotBeNull();
            result.CanUpdateGreetings.Should().BeTrue();
            result.Message.Should().Be(expected.Message);
            result.ETag.Should().Be(expected.ETag);
        }
    }
}
