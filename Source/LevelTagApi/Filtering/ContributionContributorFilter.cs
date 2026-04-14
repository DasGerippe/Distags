using DataClasses;

namespace LevelTagApi.Filtering;

public class ContributionContributorFilter : IQueryableFilter<Contribution>
{
    private readonly long _SteamId;
    private readonly bool _IgnoreAnonymity;

    public ContributionContributorFilter(long steamId, bool ignoreAnonymity)
    {
        _SteamId = steamId;
        _IgnoreAnonymity = ignoreAnonymity;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution =>
            (_IgnoreAnonymity || !contribution.User!.IsAnonymous) && contribution.User!.SteamId == _SteamId);
    }
}