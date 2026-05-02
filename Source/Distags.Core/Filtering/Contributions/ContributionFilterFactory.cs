using DataClasses;

namespace Distags.Core.Filtering.Contributions;

public class ContributionFilterFactory : IQueryableFilterFactory<Contribution, ContributionFilterParameters>
{
    public IQueryableFilter<Contribution> GetQueryableFilter(ContributionFilterParameters filterParams)
    {
        FilterGroup<Contribution> filterGroup = new FilterGroup<Contribution>();

        if (filterParams.UserSteamId != null)
            filterGroup.AddFilter(new UserFilter(filterParams.UserSteamId.Value, filterParams.IgnoreAnonymity));

        if (filterParams.AppName != null)
            filterGroup.AddFilter(new AppFilter(filterParams.AppName));

        if (filterParams.LevelId != null)
            filterGroup.AddFilter(new LevelFilter(filterParams.LevelId.Value));

        if (filterParams.TagName != null)
            filterGroup.AddFilter(new ChangedTagFilter(filterParams.TagName));

        return filterGroup;
    }
}