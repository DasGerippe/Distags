namespace Distags.Core.Commands;

public interface ICommandHandler<TCommand>
{
    Task HandleAsync(TCommand command);
}