using System;
using System.Linq;
using FluentAssertions;
using RedLine.A3;
using RedLine.Application;
using RedLine.Application.ActivityValidation;
using Xunit;

namespace RedLine.UnitTests.Application.ActivityValidation
{
    public class AuthorizableForResourceValidatorShould
    {
        private Activity Valid => new Activity
        {
            ResourceId = "Naisox",
            ResourceTypes = new[] { "First", "Second" },
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
        public void FailOnBadResourceId(string resourceId)
        {
            var activity = Valid;
            activity.ResourceId = resourceId;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Fact]
        public void FailOnNullResourceType()
        {
            var activity = Valid;
            activity.ResourceTypes = null;

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Fact]
        public void FailOnEmptyResourceType()
        {
            var activity = Valid;
            activity.ResourceTypes = Array.Empty<string>();

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void FailOnInvalidResourceType(string badResourceType)
        {
            var activity = Valid;
            activity.ResourceTypes = activity.ResourceTypes
                .Concat(new[] { badResourceType })
                .ToArray();

            new Validator()
                .Validate(activity)
                .IsValid.Should().BeFalse();
        }

        public class Validator : AuthorizableForResourceValidator<Activity>
        {
        }

        public class Activity : IAmAuthorizableForResource
        {
            public IAccessToken AccessToken { get; set; }

            public string ResourceId { get; set; }

            public string[] ResourceTypes { get; set; }
        }
    }
}
