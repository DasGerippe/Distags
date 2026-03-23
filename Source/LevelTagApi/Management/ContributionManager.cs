using DataClasses;
using DataStoring;
using System.Security.Claims;

namespace LevelTagApi.Management;

public class ContributionManager : IContributionManager
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly LevelTagDbContext _DbContext;

    public ContributionManager(IHttpContextAccessor httpContextAccessor, LevelTagDbContext dbContext)
    {
        _HttpContextAccessor = httpContextAccessor;
        _DbContext = dbContext;
    }

    public async Task CreateAddTagContributionAsync(
        long levelId, long tagId, IEnumerable<long> removedConflictingTagIds)
    {
        User? user = await GetCurrentUser();
        DateTime utcNow = DateTime.UtcNow;

        _DbContext.Add(new Contribution
        {
            LevelId = levelId,
            TagId = tagId,
            User = user,
            Action = ContributionAction.TagAdded,
            ContributedOn = utcNow,
        });

        _DbContext.AddRange(removedConflictingTagIds.Select(removedTagId => new Contribution
        {
            LevelId = levelId,
            TagId = removedTagId,
            User = user,
            Action = ContributionAction.TagRemoved,
            ContributedOn = utcNow,
        }));
    }

    public async Task CreateRemoveTagContributionAsync(long levelId, long tagId)
    {
        User? user = await GetCurrentUser();

        _DbContext.Add(new Contribution
        {
            LevelId = levelId,
            TagId = tagId,
            User = user,
            Action = ContributionAction.TagRemoved,
            ContributedOn = DateTime.UtcNow,
        });
    }

    private async Task<User?> GetCurrentUser()
    {
        ClaimsPrincipal? httpContextUser = _HttpContextAccessor.HttpContext?.User;
        if (httpContextUser == null)
            return null;

        return await _DbContext.GetUser(httpContextUser);
    }
}