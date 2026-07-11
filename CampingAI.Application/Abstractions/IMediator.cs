namespace CampingAI.Application.Abstractions;
public interface IMediator {
    Task SendCommandAsync<TCommand, TResult>(TCommand command) where TCommand : Command.ICommand;
    Task SendCommandAsync<TCommand>(TCommand command) where TCommand : Command.ICommand;
    Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query) where TQuery : Query.IQuery<TResult>;

}
