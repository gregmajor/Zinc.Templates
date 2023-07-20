using System;
using System.Transactions;
using FluentAssertions;
using RedLine.A3;
using RedLine.Application;
using RedLine.Application.ActivityValidation;
using Xunit;

namespace RedLine.UnitTests.Application.ActivityValidation
{
    public class ActivityValidatorShould
    {
        private Activity Valid => new Activity
        {
            ActivityType = ActivityType.Command,
            ActivityName = "Name",
            ActivityDisplayName = "DisplayName",
            ActivityDescription = "Description",
        };

        [Fact]
        public void HappyPath()
        {
            new Validator()
                .Validate(Valid)
                .IsValid.Should().BeTrue();
        }

        [Fact]
        public void FailOnUnknownActivityType()
        {
            var activity = Valid;
            activity.ActivityType = ActivityType.Unknown;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("   ")]
        public void FailOnInvalidActivityName(string activityName)
        {
            var activity = Valid;
            activity.ActivityName = activityName;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("   ")]
        public void FailOnInvalidActivityDisplayName(string activityDisplayName)
        {
            var activity = Valid;
            activity.ActivityDisplayName = activityDisplayName;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("   ")]
        public void FailOnInvalidActivityDescription(string activityDescription)
        {
            var activity = Valid;
            activity.ActivityDescription = activityDescription;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        public class Validator : ActivityValidator<Activity>
        {
        }

        public class Activity : IActivity
        {
            public string TenantId { get; set; }

            public Guid CorrelationId { get; set; }

            public IAccessToken AccessToken { get; set; }

            public TransactionScopeOption TransactionBehavior { get; set; }

            public IsolationLevel TransactionIsolation { get; set; }

            public TimeSpan TransactionTimeout { get; set; }

            public ActivityType ActivityType { get; set; }

            public string ActivityName { get; set; }

            public string ActivityDisplayName { get; set; }

            public string ActivityDescription { get; set; }
        }
    }
}
