
using Microsoft.Extensions.DependencyInjection;

namespace CampingAI.Application;
public class Mediator : Abstractions.IMediator {
    readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }


    public async Task SendCommandAsync<TCommand, TResult>(TCommand command) where TCommand : Abstractions.Command.ICommand {
        var handler = _serviceProvider.GetRequiredService<Abstractions.Command.ICommandHandler<TCommand, TResult>>();
        await handler.HandleAsync(command);
    }

    public async Task SendCommandAsync<TCommand>(TCommand command) where TCommand : Abstractions.Command.ICommand {
        var handler = _serviceProvider.GetRequiredService<Abstractions.Command.ICommandHandler<TCommand>>();
        await handler.HandleAsync(command);
    }

    public async Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query) where TQuery : Abstractions.Query.IQuery<TResult> {
        var handler = _serviceProvider.GetRequiredService<Abstractions.Query.IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query);
    }
}