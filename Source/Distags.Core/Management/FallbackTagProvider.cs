using Distags.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Distags.Core.Management;

public class FallbackTagProvider
{
    private readonly DistagsDbContext _DbContext;

    private IReadOnlyCollection<Tag>? _FallbackTags;

    public FallbackTagProvider(DistagsDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Tag>> GetFallbackTagsAsync()
    {
        if (_FallbackTags == null)
            _FallbackTags = (await QueryFallbackTagsAsync().ConfigureAwait(false)).AsReadOnly();

        return _FallbackTags;
    }

    private Task<List<Tag>> QueryFallbackTagsAsync()
    {
        return _DbContext.Set<Tag>().Where(tag => tag.IsFallback).ToListAsync();
    }
}