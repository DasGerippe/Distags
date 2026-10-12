using Distags.Contracts;

namespace Distags.Core.Queries.WorkshopLevelById;

public class WorkshopLevelByIdQuery : IQuery<WorkshopLevelDto>
{
    public long WorkshopId { get; set; }
}
