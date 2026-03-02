using PhotoBooth.Application.Interfaces;
using PhotoBooth.Domain;
using System.Threading.Tasks;

namespace PhotoBooth.Services
{
    public class MockPhotoValidator : Application.Interfaces.IPhotoRepository
    {
        public Task<PhotoValidationResult> ValidateAsync(Photo photo)
        {
            // MOCK validatie: altijd geldig als er een foto is
            if (photo == null)
            {
                return Task.FromResult(new PhotoValidationResult
                {
                    IsValid = false,
                    Message = "Geen foto ontvangen"
                });
            }

            return Task.FromResult(new PhotoValidationResult
            {
                IsValid = true,
                Message = "Foto voldoet aan de eisen"
            });
        }
    }
}
