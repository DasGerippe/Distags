namespace Distags.Api.Contracts.DisplayGroups;

public class DisplayGroupResponse
{
    public string Name { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Position { get; init; }
}
