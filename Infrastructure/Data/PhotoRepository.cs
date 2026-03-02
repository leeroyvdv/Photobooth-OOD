using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Interfaces;
using PhotoBooth.Domain;

namespace PhotoBooth.Infrastructure.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly AppDbContext _context;

    public PhotoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Photo photo)
    {
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Photos.CountAsync();
    }
}