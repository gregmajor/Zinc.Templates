using RedLine.Application.ActivityValidation;

namespace RedLine.Application.Notifications
{
    /// <summary>
    /// Validates a <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The type of command to validate.</typeparam>
    public abstract class NotificationValidator<TNotification> : RequestValidator<TNotification>
        where TNotification : INotification
    {
    }
}
