using Distags.Contracts;

namespace Distags.Core.Queries.AutoTaggerByGuid;

public class AutoTaggerByGuidQuery : IQuery<AutoTaggerDto?>
{
    public required Guid AutoTaggerGuid { get; init; }
}
