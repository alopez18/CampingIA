namespace CampingAI.Application.Abstractions.Command;
public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand {
    Task<TResult> HandleAsync(TCommand command);
}

public interface ICommandHandler<TCommand> where TCommand : ICommand {
    Task HandleAsync(TCommand command);
}
