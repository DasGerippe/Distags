using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class AutoTaggerFilter : IQueryableFilter<Contribution>
{
    private readonly Guid _AutoTaggerGuid;

    public AutoTaggerFilter(Guid autoTaggerGuid)
    {
        _AutoTaggerGuid = autoTaggerGuid;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.AutoTagger!.Guid == _AutoTaggerGuid);
    }
}
