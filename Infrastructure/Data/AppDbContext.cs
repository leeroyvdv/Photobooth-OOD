using Microsoft.EntityFrameworkCore;
using PhotoBooth.Domain;

namespace PhotoBooth.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Photo> Photos { get; set; }
}