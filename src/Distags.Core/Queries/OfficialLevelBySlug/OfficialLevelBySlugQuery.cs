using Distags.Contracts;

namespace Distags.Core.Queries.OfficialLevelBySlug;

public class OfficialLevelBySlugQuery : IQuery<OfficialLevelDto>
{
    public string Slug { get; set; } = string.Empty;
}
