using DataClasses;

namespace LevelTagApi.Filtering;

public class ContributionContributorFilter : IQueryableFilter<Contribution>
{
    private readonly long _SteamId;
    private readonly bool _RespectAnonymity;

    public ContributionContributorFilter(long steamId, bool respectAnonymity)
    {
        _SteamId = steamId;
        _RespectAnonymity = respectAnonymity;
    }

    public IQueryable<Contribution> Filter(IQueryable<Contribution> source)
    {
        return source.Where(contribution =>
            contribution.User!.SteamId == _SteamId && (!contribution.User!.IsAnonymous || !_RespectAnonymity));
    }
}