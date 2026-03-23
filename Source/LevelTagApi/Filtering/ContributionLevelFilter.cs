using DataClasses;

namespace LevelTagApi.Filtering;

public class ContributionLevelFilter : IQueryableFilter<Contribution>
{
    private readonly long _LevelId;

    public ContributionLevelFilter(long levelId)
    {
        _LevelId = levelId;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.Level.Id == _LevelId);
    }
}