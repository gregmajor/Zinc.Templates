using System;
using System.Linq;
using FluentAssertions;
using RedLine.Application.Jobs;
using Xunit;

namespace Zinc.Templates.UnitTests.Application.Jobs
{
    public class JobsShould : RequestsShould<IJob>
    {
        [Theory]
        [MemberData(nameof(GetRequestTypesThatShouldUseStandardValidators))]
        public void UseStandardValidators(Type commandType)
        {
            if (commandType == typeof(object))
            {
                // avoids issue where theory's member data doesn't return any types.
                return;
            }

            var validator = typeof(JobValidator<>).MakeGenericType(commandType);

            AllValidators
                .Where(t => t.IsAssignableTo(validator))
                .Should().NotBeEmpty($"Job must have a validator that inherits {typeof(JobValidator<>).FullName}");
        }
    }
}
