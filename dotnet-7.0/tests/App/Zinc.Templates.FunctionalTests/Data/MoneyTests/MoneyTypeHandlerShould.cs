using System.Data;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using RedLine.Domain;
using RedLine.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.FunctionalTests.Data.MoneyTests
{
    public class MoneyTypeHandlerShould : FunctionalTestBase
    {
        private readonly IDbConnection connection;

        public MoneyTypeHandlerShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            connection = GetRequiredService<IDbConnection>();
        }

        [Theory]
        [InlineData(1234.56789, "en-US")]
        [InlineData(2345.67891, "en-CA")]
        [InlineData(3456.78912, "fr-CA")]
        public async Task ReadAndWrite(decimal amount, string culture)
        {
            // Arrange
            var expected = new Money(amount, culture);

            // Act
            var sql = "SELECT @expected::public.redline_money as actual";
            var result = await connection.QuerySingleAsync(sql, new
            {
                expected,
            });

            var actual = result.actual as Money?;

            // Assert
            actual.Should().NotBeNull();
            actual.Value.Should().BeEquivalentTo(expected, options => options.UseMoneyEquivalency());
        }
    }
}
