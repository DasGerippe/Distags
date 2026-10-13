using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class ChangedTagFilter : IQueryableFilter<Contribution>
{
    private readonly string _TagName;

    public ChangedTagFilter(string tagName)
    {
        _TagName = tagName;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.Changes.Any(change => change.TagName == _TagName));
    }
}
