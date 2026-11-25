namespace Distags.Core.Queries.AutoTaggerStateObject;

public class AutoTaggerStateObjectQuery : IQuery<object?>
{
    public required Guid AutoTaggerGuid { get; set; }

    public required string StateKey { get; set; } = string.Empty;

    public required Type StateType { get; set; }

    public static AutoTaggerStateObjectQuery Create<TState>(Guid autoTaggerGuid, string stateKey)
    {
        return new AutoTaggerStateObjectQuery
        {
            AutoTaggerGuid = autoTaggerGuid,
            StateKey = stateKey,
            StateType = typeof(TState),
        };
    }
}
