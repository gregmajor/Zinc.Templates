using MediatR;

namespace RedLine.Application.Commands
{
    /// <summary>
    /// The interface that defines a contract for commands.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned from the command.</typeparam>
    public interface ICommand<out TResponse> : ICommand, IActivity, IRequest<TResponse>
    {
    }

    /// <summary>
    /// The marker interface for all commands.
    /// </summary>
    public interface ICommand
    {
    }
}
