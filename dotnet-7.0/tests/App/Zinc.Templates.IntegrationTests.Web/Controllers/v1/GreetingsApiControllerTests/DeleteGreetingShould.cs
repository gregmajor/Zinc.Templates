using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using RedLine.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Model;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GreetingsApiControllerTests
{
    public class DeleteGreetingShould : WebTestBase
    {
        private static readonly string Endpoint = $"/api/v1/{TenantId}/greetings";

        private readonly IRepository<Greeting> repository;

        public DeleteGreetingShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            repository = GetRequiredService<IRepository<Greeting>>();
        }

        [Fact]
        public async Task DeleteTheGreeting()
        {
            // Arrange
            var expected = GreetingMother.HiThere();
            await repository.Save(expected).ConfigureAwait(false);

            // Act
            await AuthorizedScenario(_ =>
            {
                _.WithRequestHeader(HeaderNames.IfMatch, expected.ETag);
                _.Delete.Url(string.Join('/', Endpoint, expected.Key));

                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnConcurrencyError()
        {
            // Arrange
            var expected = GreetingMother.HiThere();
            await repository.Save(expected).ConfigureAwait(false);

            // Act
            await AuthorizedScenario(_ =>
            {
                _.WithRequestHeader(HeaderNames.IfMatch, "wrong-etag");
                _.Delete.Url(string.Join('/', Endpoint, expected.Key));

                _.StatusCodeShouldBe(412);
            }).ConfigureAwait(false);
        }
    }
}
