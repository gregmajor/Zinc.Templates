using System;
using System.Linq;
using FluentAssertions;
using RedLine.Application.Notifications;
using Xunit;

namespace Zinc.Templates.UnitTests.Application.Notifications
{
    public class NotificationsShould : RequestsShould<INotification>
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

            var validator = typeof(NotificationValidator<>).MakeGenericType(commandType);

            AllValidators
                .Where(t => t.IsAssignableTo(validator))
                .Should().NotBeEmpty($"Notification must have a validator that inherits {typeof(NotificationValidator<>).FullName}");
        }
    }
}
