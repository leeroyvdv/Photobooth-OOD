using PhotoBooth.Domain;

namespace PhotoBooth.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoValidationResult> ValidateAndSaveAsync(byte[] data, string contentType);
}