using DataClasses;

namespace LevelTagApi.Filtering;

public class ContributionTagFilter : IQueryableFilter<Contribution>
{
    private readonly string _TagName;

    public ContributionTagFilter(string tagName)
    {
        _TagName = tagName;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.Tag.Name == _TagName);
    }
}