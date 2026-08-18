using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class LevelFilter : IQueryableFilter<Contribution>
{
    private readonly long _LevelId;

    public LevelFilter(long levelId)
    {
        _LevelId = levelId;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.Level.Id == _LevelId);
    }
}
