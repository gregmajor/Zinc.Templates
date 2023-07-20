using FluentAssertions;
using RedLine.Application;
using RedLine.Application.ActivityValidation;
using Xunit;

namespace RedLine.UnitTests.Application.ActivityValidation
{
    public class MultiTenantValidatorShould
    {
        private Activity Valid => new Activity
        {
            TenantId = "Naisox",
        };

        [Fact]
        public void HappyPath()
        {
            new Validator()
                .Validate(Valid)
                .IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void FailOnBadTenantId(string tenantId)
        {
            var activity = Valid;
            activity.TenantId = tenantId;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        public class Validator : MultiTenantValidator<Activity>
        {
        }

        public class Activity : IAmMultiTenant
        {
            public string TenantId { get; set; }
        }
    }
}
