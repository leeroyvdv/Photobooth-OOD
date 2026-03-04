using PhotoBooth.Application.Interfaces;
using PhotoBooth.Domain;

namespace PhotoBooth.Application.Services;

// Service die de foto laat controleren en opslaat als hij geldig is.
public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _repository;
    private readonly PhotoValidationService _validator;

    public PhotoService(
        IPhotoRepository repository,
        PhotoValidationService validator)
    {
        // Repository wordt gebruikt om foto's op te slaan in de database
        _repository = repository;

        // Validator controleert of de foto aan de eisen voldoet
        _validator = validator;
    }

    public async Task<PhotoValidationResult> ValidateAndSaveAsync(
        byte[] data,
        string contentType)
    {
        // Eerst controleren of de foto geldig is
        var validation = _validator.Validate(data);

        // Als de foto niet geldig is stoppen we hier
        if (!validation.IsValid)
            return validation;

        // Als de foto wel geldig is maken we een Photo object
        var photo = new Photo(data, contentType);

        // Daarna slaan we de foto op via de repository
        await _repository.AddAsync(photo);

        // Resultaat van de validatie teruggeven aan de controller
        return validation;
    }
}