namespace Distags.Api.Contracts.DisplayGroups;

public class DisplayGroupResponse
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Position { get; set; }
}
