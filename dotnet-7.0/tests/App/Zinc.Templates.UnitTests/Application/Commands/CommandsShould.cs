using System;
using System.Linq;
using FluentAssertions;
using RedLine.Application.Commands;
using Xunit;

namespace Zinc.Templates.UnitTests.Application.Commands
{
    public class CommandsShould : RequestsShould<ICommand>
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

            var validator = typeof(CommandValidator<>).MakeGenericType(commandType);

            AllValidators
                .Where(t => t.IsAssignableTo(validator))
                .Should().NotBeEmpty($"Command must have a validator that inherits {typeof(CommandValidator<>).FullName}");
        }

        [Theory]
        [MemberData(nameof(GetRequestTypesThatShouldUseStandardValidators))]
        public void FollowNamingConvention(Type commandType)
        {
            if (commandType == typeof(object))
            {
                // avoids issue where theory's member data doesn't return any types.
                return;
            }

            commandType.Name.Should().EndWith("Command");
        }
    }
}
