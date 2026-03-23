using DataClasses;
using DataStoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace LevelTagApi.Security;

public class PostAutoTagHandler : AuthorizationHandler<PostAutoTagRequirement>
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly LevelTagDbContext _DbContext;

    public PostAutoTagHandler(IHttpContextAccessor httpContextAccessor, LevelTagDbContext dbContext)
    {
        _HttpContextAccessor = httpContextAccessor;
        _DbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PostAutoTagRequirement requirement)
    {
        HttpContext? httpContext = _HttpContextAccessor.HttpContext;
        if (httpContext == null)
            return;

        if (!httpContext.Request.RouteValues.TryGetValue("tagName", out object? tagNameValue))
            return;

        string? tagName = tagNameValue?.ToString();
        Tag? tag = await _DbContext.Set<Tag>().FirstOrDefaultAsync(tag => tag.Name == tagName);
        if (tag == null)
            return;

        bool userCanPostTag = !tag.IsAutoTag || context.User.IsInRole(Constants.Roles.AutoTagger);
        if (userCanPostTag)
            context.Succeed(requirement);
    }
}