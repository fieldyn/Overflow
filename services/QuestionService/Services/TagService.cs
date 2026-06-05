using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuestionService.Data;
using QuestionService.Models;

namespace QuestionService.Services;

public class TagService(IMemoryCache cache, QuestionDbContext db)
{
    private const string CacheKey = "tags";

    public async Task<List<Tag>> GetTags()
    {
        return await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
            var tags = await db.Tags.AsNoTracking().ToListAsync();
            return tags;
        }) ?? [];
    }
    
    public async Task<bool> AreTagsValidAsync(List<string> tagSlugs)
    {
        var tags = await GetTags();
        var validSlugs = tags.Select(x => x.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return tagSlugs.All(slug => validSlugs.Contains(slug));
    }
}
