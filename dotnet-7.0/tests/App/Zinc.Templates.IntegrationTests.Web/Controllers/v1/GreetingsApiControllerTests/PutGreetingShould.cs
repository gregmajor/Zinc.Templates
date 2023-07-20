using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using RedLine.Data.Outbox;
using RedLine.Domain.Repositories;
using Xunit;
using Xunit.Abstractions;
using Zinc.Templates.Application.Commands.PutGreeting;
using Zinc.Templates.Domain;
using Zinc.Templates.Domain.Events;
using Zinc.Templates.Domain.Model;
using Zinc.Templates.IntegrationTests.Web.Mothers;

namespace Zinc.Templates.IntegrationTests.Web.Controllers.V1.GreetingsApiControllerTests
{
    public class PutGreetingShould : WebTestBase
    {
        private static readonly string Endpoint = $"/api/v1/{TenantId}/greetings";

        private readonly IRepository<Greeting> repository;

        public PutGreetingShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            repository = GetRequiredService<IRepository<Greeting>>();
        }

        [Fact]
        public async Task SaveTheModel()
        {
            PutGreetingModel model = new() { GreetingId = Guid.NewGuid(), Message = "Hello there" };

            await AuthorizedScenario(_ =>
            {
                _.Put.Json(model).ToUrl(Endpoint);

                _.StatusCodeShouldBe(201);
                _.Header(HeaderNames.ETag).ShouldHaveOneNonNullValue();
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task UpdateTheModel()
        {
            // Arrange
            var existing = GreetingMother.HiThere();
            await repository.Save(existing).ConfigureAwait(false);
            PutGreetingModel model = new() { GreetingId = existing.GreetingId, Message = "Hello there" };

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.WithRequestHeader(HeaderNames.IfMatch, existing.ETag);
                _.Put.Json(model).ToUrl(Endpoint);

                _.StatusCodeShouldBe(201);
                _.Header(HeaderNames.ETag).ShouldHaveOneNonNullValue();
            }).ConfigureAwait(false);

            // Assert
            response.Context.Response.Headers[HeaderNames.ETag][0].Should().NotBe(existing.ETag);

            var db = GetRequiredService<IDbConnection>();
            var record = await db.QueryFirstAsync<OutboxRecord>("select * from outbox.outbox").ConfigureAwait(false);
            record.Messages.Count().Should().Be(1);
            var first = record.Messages.First();
            first.MessageBody.Should().BeOfType<GreetingMessageChanged>();

            var domainEvent = (GreetingMessageChanged)first.MessageBody;
            domainEvent.GreetingId.Should().Be(existing.GreetingId);
            domainEvent.PreviousMessage.Should().Be(existing.Message);
            domainEvent.NewMessage.Should().Be(model.Message);
        }

        [Fact]
        public async Task ReturnConcurrencyError()
        {
            // Arrange
            var existing = GreetingMother.HiThere();
            await repository.Save(existing).ConfigureAwait(false);
            PutGreetingModel model = new() { GreetingId = existing.GreetingId, Message = "Hello there" };

            // Act
            await AuthorizedScenario(_ =>
            {
                _.WithRequestHeader(HeaderNames.IfMatch, "wrong-etag");
                _.Put.Json(model).ToUrl(Endpoint);

                _.StatusCodeShouldBe(412);
                _.Header(HeaderNames.ETag).ShouldHaveOneNonNullValue();
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnNotAuthenticated()
        {
            await AnonymousScenario(_ =>
            {
                _.Put.Url(Endpoint);

                _.StatusCodeShouldBe(401);
            }).ConfigureAwait(false);
        }
    }
}
