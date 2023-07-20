using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using RedLine.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Application.Queries.GetGreeting;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GreetingsApiControllerTests
{
    public class GetGreetingShould : WebTestBase
    {
        private static readonly string Endpoint = $"/api/v1/{TenantId}/greetings";

        private readonly IRepository<Greeting> repository;

        public GetGreetingShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            repository = GetRequiredService<IRepository<Greeting>>();
        }

        [Fact]
        public async Task ReturnGreeting()
        {
            // Arrange
            var expected = GreetingMother.HiThere();
            await repository.Save(expected).ConfigureAwait(false);

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(string.Join('/', Endpoint, expected.Key));

                _.StatusCodeShouldBe(200);
                _.Header(HeaderNames.ETag).SingleValueShouldEqual(expected.ETag);
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<GetGreetingResult>();
            result.Should().NotBeNull();
            result.GreetingId.Should().Be(expected.Key);
            result.Message.Should().Be(expected.Message);
        }

        [Fact]
        public async Task ReturnNotAuthenticated()
        {
            await AnonymousScenario(_ =>
            {
                _.Get.Url(string.Join('/', Endpoint, Guid.NewGuid().ToString()));

                _.StatusCodeShouldBe(401);
            }).ConfigureAwait(false);
        }
    }
}
