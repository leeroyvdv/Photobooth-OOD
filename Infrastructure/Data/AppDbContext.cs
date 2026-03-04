using Microsoft.EntityFrameworkCore;
using PhotoBooth.Domain;

namespace PhotoBooth.Infrastructure.Data;

// DbContext regelt de verbinding met de database
public class AppDbContext : DbContext
{
    // Constructor die de database instellingen ontvangt
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Tabel in de database voor Photo objecten
    // Entity Framework maakt hier automatisch een tabel van
    public DbSet<Photo> Photos { get; set; }
}