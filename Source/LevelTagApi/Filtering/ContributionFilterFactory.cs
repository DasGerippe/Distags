using DataClasses;

namespace LevelTagApi.Filtering;

public class ContributionFilterFactory : IQueryableFilterFactory<Contribution, ContributionFilterParameters>
{
    public IQueryableFilter<Contribution> GetQueryableFilter(ContributionFilterParameters filterParams)
    {
        FilterGroup<Contribution> filterGroup = new FilterGroup<Contribution>();

        if (filterParams.UserSteamId != null)
            filterGroup.AddFilter(new ContributionContributorFilter(filterParams.UserSteamId.Value, filterParams.RespectAnonymity));

        if (filterParams.LevelId != null)
            filterGroup.AddFilter(new ContributionLevelFilter(filterParams.LevelId.Value));

        if (filterParams.TagName != null)
            filterGroup.AddFilter(new ContributionTagFilter(filterParams.TagName));

        return filterGroup;
    }
}