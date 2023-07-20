using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Zinc.Templates.IntegrationTests.Messaging.HealthCheckTests
{
    public class HealthCheckShould : MessagingTestBase
    {
        public HealthCheckShould(MessagingTestFixture fixture, ITestOutputHelper output)
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
            // 1. Working Set Memory should be below 2GB
            // 2. Process allocated memory should be below 2GB
            // 3. AuthZ Service
            // 4. AuthN Certificate
            // 5. Database
            // 6. RabbitMq connection
            // 7. Overall Status

            Regex.Matches(resultText, "Healthy").Count.Should().Be(7);
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
