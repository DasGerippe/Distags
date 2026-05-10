using Distags.Core.Commands;
using Distags.Core.Queries;

namespace Distags.Core;

public interface IRequestDispatcher
{
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);

    Task DispatchAsync(ICommand command);
}