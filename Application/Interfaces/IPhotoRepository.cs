using PhotoBooth.Domain;

namespace PhotoBooth.Application.Interfaces;

// Interface voor de PhotoRepository.
// Hierin staat welke dingen een repository voor foto's moet kunnen doen.
public interface IPhotoRepository
{
    // Slaat een foto op in de database.
    // Async omdat database acties even kunnen duren.
    Task AddAsync(Photo photo);

    // Geeft terug hoeveel foto's er in totaal zijn opgeslagen.
    Task<int> CountAsync();
}