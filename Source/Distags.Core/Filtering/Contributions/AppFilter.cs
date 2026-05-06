using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class AppFilter : IQueryableFilter<Contribution>
{
    private readonly string _AppName;

    public AppFilter(string appName)
    {
        _AppName = appName;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution => contribution.App!.Name == _AppName);
    }
}