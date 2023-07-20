using System;
using FluentAssertions;
using RedLine.Application;
using RedLine.Application.ActivityValidation;
using Xunit;

namespace RedLine.UnitTests.Application.ActivityValidation
{
    public class CorrelatableValidatorShould
    {
        private Activity Valid => new Activity
        {
            CorrelationId = Guid.NewGuid(),
        };

        [Fact]
        public void HappyPath()
        {
            new Validator()
                .Validate(Valid)
                .IsValid.Should().BeTrue();
        }

        [Fact]
        public void FailOnEmptyCorrelationId()
        {
            var activity = Valid;
            activity.CorrelationId = Guid.Empty;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        public class Validator : CorrelatableValidator<Activity>
        {
        }

        public class Activity : IAmCorrelatable
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
