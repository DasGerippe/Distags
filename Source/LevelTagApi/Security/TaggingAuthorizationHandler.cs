using DataClasses;
using DataStoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LevelTagApi.Security;

public class TaggingAuthorizationHandler : AuthorizationHandler<TaggingAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _HttpContextAccessor;
    private readonly LevelTagDbContext _DbContext;

    public TaggingAuthorizationHandler(IHttpContextAccessor httpContextAccessor, LevelTagDbContext dbContext)
    {
        _HttpContextAccessor = httpContextAccessor;
        _DbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TaggingAuthorizationRequirement requirement)
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

        ClaimsPrincipal user = context.User;

        bool canModifyLevelTag =
            !tag.IsAutoTag && user.IsInRole(Constants.Roles.Tagger) ||
            tag.IsAutoTag && user.IsInRole(Constants.Roles.AutoTagger);

        if (canModifyLevelTag)
            context.Succeed(requirement);
    }
}