using Distags.Core.Entities;

namespace Distags.Core.Filtering.Contributions;

public class ContributionFilterFactory : IQueryableFilterFactory<Contribution, ContributionFilterParameters>
{
    private readonly IStringContainsFilterFactory _StringContainsFilterFactory;

    public ContributionFilterFactory(IStringContainsFilterFactory stringContainsFilterFactory)
    {
        _StringContainsFilterFactory = stringContainsFilterFactory;
    }

    public IQueryableFilter<Contribution> GetQueryableFilter(ContributionFilterParameters filterParams)
    {
        FilterGroup<Contribution> filterGroup = new FilterGroup<Contribution>();

        if (filterParams.UserSteamId != null)
            filterGroup.AddFilter(new UserFilter(filterParams.UserSteamId.Value, filterParams.IgnoreAnonymity));

        if (filterParams.AutoTaggerGuid != null)
            filterGroup.AddFilter(new AutoTaggerFilter(filterParams.AutoTaggerGuid.Value));

        if (filterParams.LevelSlug != null)
            filterGroup.AddFilter(_StringContainsFilterFactory.CreateFilter<Contribution>(
                contribution => (contribution.Level as OfficialLevel)!.Slug,
                filterParams.LevelSlug));

        if (filterParams.LevelWorkshopId != null)
            filterGroup.AddFilter(new WorkshopLevelFilter(filterParams.LevelWorkshopId.Value));

        if (filterParams.TagName != null)
            filterGroup.AddFilter(new ChangedTagFilter(filterParams.TagName));

        if (filterParams.ContributedAfter != null)
            filterGroup.AddFilter(new SimpleFilter<Contribution>(contribution => contribution.ContributedOn > filterParams.ContributedAfter));

        if (filterParams.ContributedBefore != null)
            filterGroup.AddFilter(new SimpleFilter<Contribution>(contribution => contribution.ContributedOn < filterParams.ContributedBefore));

        return filterGroup;
    }
}
