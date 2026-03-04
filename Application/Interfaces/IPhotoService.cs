using PhotoBooth.Domain;

namespace PhotoBooth.Application.Interfaces;

// Interface voor de PhotoService.
// Deze service regelt het controleren van de foto en het opslaan ervan.
public interface IPhotoService
{
    // Deze methode controleert een foto en slaat hem op als hij geldig is.
    // data = de foto zelf (als bytes)
    // contentType = het type afbeelding, bijvoorbeeld image/png
    // Geeft een resultaat terug met info of de foto goed of afgekeurd is.
    Task<PhotoValidationResult> ValidateAndSaveAsync(byte[] data, string contentType);
}