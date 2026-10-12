using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class WorkshopLevelFilter : IQueryableFilter<Contribution>
{
    private readonly long _LevelWorkshopId;

    public WorkshopLevelFilter(long levelWorkshopId)
    {
        _LevelWorkshopId = levelWorkshopId;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => (contribution.Level as WorkshopLevel)!.WorkshopId == _LevelWorkshopId);
    }
}
