using PhotoBooth.Domain;

namespace PhotoBooth.Application.Services;

public class PhotoValidationService
{
    public PhotoValidationResult Validate(byte[] data)
    {
        bool isValid = data.Length > 0;

        return new PhotoValidationResult(
            isValid,
            faceDetected: true,
            eyesVisible: true,
            neutralExpression: true,
            lightingOk: true
        );
    }
}