using RedLine.Application.ActivityValidation;

namespace RedLine.Application.Commands
{
    /// <summary>
    /// Validates a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to validate.</typeparam>
    public abstract class CommandValidator<TCommand> : RequestValidator<TCommand>
        where TCommand : ICommand
    {
    }
}
