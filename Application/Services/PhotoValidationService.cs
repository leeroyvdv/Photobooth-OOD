using PhotoBooth.Domain;

namespace PhotoBooth.Application.Services;

// Service die controleert of een foto geldig is
public class PhotoValidationService
{
    public PhotoValidationResult Validate(byte[] data)
    {
        // Simpele check: kijken of er überhaupt data in de foto zit
        bool isValid = data.Length > 0;

        // Hier wordt het resultaat van de controle gemaakt
        // In een echte situatie zou hier AI of beeldherkenning zitten
        return new PhotoValidationResult(
            isValid,
            faceDetected: true,
            eyesVisible: true,
            neutralExpression: true,
            lightingOk: true
        );
    }
}