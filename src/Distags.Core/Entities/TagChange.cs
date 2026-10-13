using Distags.Contracts;

namespace Distags.Core.Entities;

public class TagChange
{
    public TagChangeAction Action { get; set; }

    public string TagName { get; set; } = string.Empty;
}
