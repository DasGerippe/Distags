using Distags.Core;
using Distags.Core.Commands.SetAutoTaggerStateObject;
using Distags.Core.Queries.AutoTaggerStateObject;

namespace AutoTagging.Abstractions;

public class StateStore
{
    private readonly AutoTaggerInfo _AutoTaggerInfo;
    private readonly IRequestDispatcher _RequestDispatcher;

    public StateStore(AutoTaggerInfo autoTaggerInfo, IRequestDispatcher requestDispatcher)
    {
        _AutoTaggerInfo = autoTaggerInfo;
        _RequestDispatcher = requestDispatcher;
    }

    public async Task<TState> GetStateAsync<TState>(string stateKey)
        where TState : class, new()
    {
        AutoTaggerStateObjectQuery query = AutoTaggerStateObjectQuery.Create<TState>(_AutoTaggerInfo.Guid, stateKey);
        try
        {
            return await _RequestDispatcher.DispatchAsync(query).ConfigureAwait(false) as TState ?? new TState();
        }
        catch
        {
            return new TState();
        }
    }

    public async Task SetStateAsync<TState>(string stateKey, TState? state)
        where TState : class, new()
    {
        SetAutoTaggerStateObjectCommand command = SetAutoTaggerStateObjectCommand.Create(_AutoTaggerInfo.Guid, stateKey, state);
        try
        {
            await _RequestDispatcher.DispatchAsync(command).ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }
    }
}
