using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Application.Queries.FindGreetings;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GreetingsApiControllerTests
{
    public class FindGreetingsShould : WebTestBase
    {
        private static readonly string Endpoint = $"/api/v1/{TenantId}/greetings";

        private readonly IRepository<Greeting> repository;

        public FindGreetingsShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            repository = GetRequiredService<IRepository<Greeting>>();
        }

        [Fact]
        public async Task ReturnGreetings()
        {
            // Arrange
            var hi = GreetingMother.HiThere();
            await repository.Save(hi).ConfigureAwait(false);
            var hello = GreetingMother.HelloThere();
            await repository.Save(hello).ConfigureAwait(false);

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{Endpoint}?searchPattern=%there");

                _.StatusCodeShouldBe(200);
                _.Header(HeaderNames.ETag).ShouldHaveValues($"{hi.Key}:{hi.ETag}", $"{hello.Key}:{hello.ETag}");
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<PageableResult<FindGreetingsResult>>();

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            var found = result.Items[0];
            found.GreetingId.Should().Be(hi.Key);
            found.Message.Should().Be(hi.Message);
            found = result.Items[1];
            found.GreetingId.Should().Be(hello.Key);
            found.Message.Should().Be(hello.Message);
        }
    }
}
