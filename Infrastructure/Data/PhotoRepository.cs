using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Interfaces;
using PhotoBooth.Domain;

namespace PhotoBooth.Infrastructure.Data;

// Deze class voert de database acties uit voor foto's
public class PhotoRepository : IPhotoRepository
{
    private readonly AppDbContext _context;

    public PhotoRepository(AppDbContext context)
    {
        // DbContext gebruiken om met de database te werken
        _context = context;
    }

    public async Task AddAsync(Photo photo)
    {
        // Foto toevoegen aan de Photos tabel
        _context.Photos.Add(photo);

        // Wijzigingen opslaan in de database
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        // Telt hoeveel foto's er in de database staan
        return await _context.Photos.CountAsync();
    }
}