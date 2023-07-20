using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Jobs.HealthCheckTests
{
    public class HealthCheckShould : JobsTestBase
    {
        public HealthCheckShould(JobsTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnHealthy()
        {
            var result = await AnonymousScenario(_ =>
            {
                _.Get.Url("/.well-known/ready");
                _.StatusCodeShouldBe(200);
            }).ConfigureAwait(false);

            var resultText = result.ReadAsText();

            resultText.Should().NotBeEmpty();

            // The status checks should be:
            // 1. OutboxJob
            // 2. Working Set Memory should be below 2GB
            // 3. Process allocated memory should be below 2GB
            // 4. AuthZ Service
            // 5. AuthN Certificate
            // 6. Database
            // 7. RabbitMq connection
            // 8. Overall Status

            Regex.Matches(resultText, "Healthy").Count.Should().Be(8);
        }

        [Fact]
        public async Task ReturnLivenessHealthy()
        {
            var result = await AnonymousScenario(_ =>
            {
                _.Get.Url("/.well-known/live");
                _.StatusCodeShouldBe(200);
            }).ConfigureAwait(false);

            var resultText = result.ReadAsText();

            resultText.Should().Be("pong");
        }
    }
}
