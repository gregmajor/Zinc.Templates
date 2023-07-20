using System.Threading.Tasks;
using FluentAssertions;
using RedLine.A3.Authorization.Domain;
using RedLine.Data.A3.Authorization;
using RedLine.Domain.A3.Authorization.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.A3.GrantRepositoryTests
{
    public class ExistShould : FunctionalTestBase
    {
        public ExistShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnTrue()
        {
            var scope = new GrantScope("tenant", "type", "qualifier");
            var grant = new Grant("user1", "user one", scope, null, "system");

            var repository = GetRequiredService<IGrantRepository>();

            await repository.Save(grant).ConfigureAwait(false);

            var actual = await repository.Exists(grant.Key).ConfigureAwait(false);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnFalse()
        {
            var scope = new GrantScope("tenant", "type", "qualifier");
            var grant = new Grant("user1", "user one", scope, null, "system");

            var repository = GetRequiredService<IGrantRepository>();

            await repository.Save(grant).ConfigureAwait(false);

            var actual = await repository.Exists(new GrantKey("XXX", "tenant", "type", "qualifier").ToString()).ConfigureAwait(false);

            actual.Should().BeFalse();
        }
    }
}
