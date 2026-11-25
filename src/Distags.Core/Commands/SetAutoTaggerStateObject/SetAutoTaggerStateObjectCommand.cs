namespace Distags.Core.Commands.SetAutoTaggerStateObject;

public class SetAutoTaggerStateObjectCommand : ICommand
{
    public required Guid AutoTaggerGuid { get; set; }

    public required string StateKey { get; set; } = string.Empty;

    public required Type StateType { get; set; }

    public object? StateValue { get; set; }

    public static SetAutoTaggerStateObjectCommand Create<TState>(Guid autoTaggerGuid, string stateKey, TState? stateValue)
    {
        return new SetAutoTaggerStateObjectCommand
        {
            AutoTaggerGuid = autoTaggerGuid,
            StateKey = stateKey,
            StateType = typeof(TState),
            StateValue = stateValue,
        };
    }
}
