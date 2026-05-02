using Microsoft.EntityFrameworkCore;

namespace Distags.Core;

public abstract class DistagsDbContext : DbContext
{
    protected DistagsDbContext(DbContextOptions options)
        : base(options)
    {
    }
}