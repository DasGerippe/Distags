using Distags.Contracts;

namespace Distags.Core.Queries.LevelById;

public class LevelByIdQuery : IQuery<LevelDto>
{
    public long LevelId { get; set; }
}