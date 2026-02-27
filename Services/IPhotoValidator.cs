using PhotoBooth.Domain;

namespace PhotoBooth.Services;

public interface IPhotoValidator
{
    Task<PhotoValidationResult> ValidateAsync(Photo photo);
}
