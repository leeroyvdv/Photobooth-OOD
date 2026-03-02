using PhotoBooth.Domain;

namespace PhotoBooth.Application.Interfaces;

public interface IPhotoRepository
{
    Task AddAsync(Photo photo);
    Task<int> CountAsync();
}