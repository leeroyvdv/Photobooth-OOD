using PhotoBooth.Application.Interfaces;
using PhotoBooth.Domain;

namespace PhotoBooth.Application.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _repository;
    private readonly PhotoValidationService _validator;

    public PhotoService(
        IPhotoRepository repository,
        PhotoValidationService validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<PhotoValidationResult> ValidateAndSaveAsync(
        byte[] data,
        string contentType)
    {
        var validation = _validator.Validate(data);

        if (!validation.IsValid)
            return validation;

        var photo = new Photo(data, contentType);
        await _repository.AddAsync(photo);

        return validation;
    }
}