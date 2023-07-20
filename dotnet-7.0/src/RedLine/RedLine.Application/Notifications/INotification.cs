namespace RedLine.Application.Notifications
{
    /// <summary>
    /// The interface that defines a contract for application notifications, which are similar to events.
    /// </summary>
    public interface INotification : IActivity, MediatR.INotification
    {
    }
}
