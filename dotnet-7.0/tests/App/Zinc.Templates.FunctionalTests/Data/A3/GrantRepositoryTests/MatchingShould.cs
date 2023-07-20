using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RedLine.A3.Authorization.Domain;
using RedLine.Domain.A3.Authorization.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.A3.GrantRepositoryTests
{
    public class MatchingShould : FunctionalTestBase
    {
        public MatchingShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnRecordsMatching()
        {
            var scope = new GrantScope("tenant", "type", "qualifier");

            var grants = new[]
            {
                new Grant("user1", "user one", scope, null, "system"),
                new Grant("user2", "user one", scope, null, "system"),
            };

            var repository = GetRequiredService<IGrantRepository>();

            foreach (var grant in grants)
            {
                await repository.Save(grant).ConfigureAwait(false);
            }

            var expected = new[] { grants.First() };

            var actual = await repository.Matching(scope, "user1").ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ReturnRecordsMatchingWildcards()
        {
            var scope1 = new GrantScope("tenant", "type", "qualifier1");
            var scope2 = new GrantScope("tenant", "type", "qualifier2");

            var expected = new[]
            {
                new Grant("user1", "user one", scope1, null, "system"),
                new Grant("user2", "user one", scope2, null, "system"),
            };

            var repository = GetRequiredService<IGrantRepository>();

            foreach (var grant in expected)
            {
                await repository.Save(grant).ConfigureAwait(false);
            }

            var actual = await repository.Matching(new GrantScope("ten%", "typ%", "qua%"), null).ConfigureAwait(false);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task NotReturnRecordsNotMatching()
        {
            var scope1 = new GrantScope("tenant1", "type1", "qualifier1");
            var scope2 = new GrantScope("tenant2", "type2", "qualifier2");

            var expected = new[]
            {
                new Grant("user1", "user one", scope1, null, "system"),
                new Grant("user2", "user one", scope2, null, "system"),
            };

            var repository = GetRequiredService<IGrantRepository>();

            expected.Select(async grant => await repository.Save(grant).ConfigureAwait(false));

            var actual = await repository.Matching(new GrantScope("foo", "bar", "bam"), null).ConfigureAwait(false);

            actual.Should().BeEmpty();
        }
    }
}
