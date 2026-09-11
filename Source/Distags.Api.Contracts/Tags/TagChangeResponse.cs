using Distags.Contracts;

namespace Distags.Api.Contracts.Tags;

public class TagChangeResponse
{
    public TagChangeAction Action { get; init; }

    public string TagName { get; init; } = string.Empty;
}
