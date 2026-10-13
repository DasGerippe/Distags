namespace Distags.Api.Contracts.Tags;

public class TagResponse
{
    public string Name { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string DisplayGroupName { get; init; } = string.Empty;

    public int Position { get; init; }

    public string? LogicGroupName { get; init; } = string.Empty;

    public bool IsExclusive { get; init; }

    public bool IsFallback { get; init; }

    public bool IsAutoTag { get; init; }
}
