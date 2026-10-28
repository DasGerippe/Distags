using Distags.Contracts;
using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Queries.StatsSummary;

public class StatsSummaryQueryHandler : IQueryHandler<StatsSummaryQuery, StatsSummaryDto>
{
    private readonly DistagsDbContext _DbContext;

    public StatsSummaryQueryHandler(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<StatsSummaryDto> HandleAsync(StatsSummaryQuery query)
    {
        StatsSummaryDto statsSummary = new StatsSummaryDto
        {
            LevelTagCount = await _DbContext.Set<LevelTag>().CountAsync().ConfigureAwait(false),
            LevelCount = await _DbContext.Set<Level>().CountAsync().ConfigureAwait(false),
            UserContributionCount = await GetUserContributionCount().ConfigureAwait(false),
            ContributingUserCount = await GetContributingUserCount().ConfigureAwait(false),
            AutoTaggerContributionCount = await GetAutoTaggerContributionCount().ConfigureAwait(false),
            ContributingAutoTaggerCount = await GetContributingAutoTaggerCount().ConfigureAwait(false),
        };

        return statsSummary;
    }

    private Task<int> GetUserContributionCount()
    {
        return _DbContext.Set<Contribution>().CountAsync(contribution => contribution.UserId != null);
    }

    private Task<int> GetContributingUserCount()
    {
        return _DbContext.Set<Contribution>()
            .Where(contribution => contribution.UserId != null)
            .Select(contribution => contribution.UserId)
            .Distinct()
            .CountAsync();
    }

    private Task<int> GetAutoTaggerContributionCount()
    {
        return _DbContext.Set<Contribution>().CountAsync(contribution => contribution.AutoTaggerId != null);
    }

    private Task<int> GetContributingAutoTaggerCount()
    {
        return _DbContext.Set<Contribution>()
            .Where(contribution => contribution.AutoTaggerId != null)
            .Select(contribution => contribution.AutoTaggerId)
            .Distinct()
            .CountAsync();
    }
}
