using Distags.Core.Commands;
using Distags.Core.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Distags.Core;

public class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceProvider _ServiceProvider;

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _ServiceProvider = serviceProvider;
    }

    public Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
    {
        Type handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic handler = _ServiceProvider.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)query);
    }

    public Task DispatchAsync(ICommand command)
    {
        Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        dynamic handler = _ServiceProvider.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)command);
    }
}